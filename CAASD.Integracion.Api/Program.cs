using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;

using CAASD.Integracion;
using CAASD.Integracion.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Infra ----------------------
builder.Services.AddDbContext<IntegracionDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("IntegracionConnection")));

builder.Services.AddMemoryCache();

// HttpClient hacia Core.Api (x-api-key) con handler resiliente propio
builder.Services.AddTransient<ResilienceHandler>(); // registra handler

builder.Services.AddHttpClient("core", (sp, c) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Core:BaseUrl"]!;
    c.BaseAddress = new Uri(baseUrl);

    var apiKey = cfg["Core:ApiKey"];
    if (!string.IsNullOrWhiteSpace(apiKey))
        c.DefaultRequestHeaders.Add("x-api-key", apiKey);

    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<ResilienceHandler>();  // usa nuestro handler

// HttpClient hacia Web (para enviarle admins sincronizados)
builder.Services.AddHttpClient("web", (sp, c) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    c.BaseAddress = new Uri(cfg["Web:BaseUrl"]!);
    var internalKey = cfg["Web:ApiKey"];
    if (!string.IsNullOrWhiteSpace(internalKey))
        c.DefaultRequestHeaders.Add("x-internal-key", internalKey);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


// Worker que despacha outbox
builder.Services.AddHostedService<OutboxDispatcher>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CAASD.Integracion.Api", Version = "v1" });
});

// Logging consola
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o =>
{
    o.SingleLine = true;
    o.TimestampFormat = "HH:mm:ss ";
});

var app = builder.Build();

// Mostrar URLs al iniciar
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine($"Now listening on: {string.Join(", ", app.Urls)}");
});

app.UseSwagger();
app.UseSwaggerUI();

// JSON options (coincidir con Core si usa camelCase)
var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

// ---------------------- Helpers (local functions) ----------------------
async Task<IResult> TryProxyGetAsync(
    string corePath,
    IHttpClientFactory http,
    IMemoryCache cache,
    string? cacheKey = null,
    TimeSpan? ttl = null,
    CancellationToken ct = default)
{
    if (!string.IsNullOrWhiteSpace(cacheKey) && cache.TryGetValue(cacheKey, out string cachedJson))
        return Results.Content(cachedJson, "application/json");

    var client = http.CreateClient("core");
    var res = await client.GetAsync(corePath, ct);

    if (res.StatusCode == HttpStatusCode.NotFound)
        return Results.NotFound();

    res.EnsureSuccessStatusCode();
    var json = await res.Content.ReadAsStringAsync(ct);

    if (!string.IsNullOrWhiteSpace(cacheKey))
        cache.Set(cacheKey!, json, ttl ?? TimeSpan.FromSeconds(60));

    return Results.Content(json, "application/json");
}

async Task<IResult> TryProxyPostAsync(
    string corePath,
    object body,
    IHttpClientFactory http,
    IntegracionDbContext db,
    string outboxType,
    CancellationToken ct = default)
{
    try
    {
        var client = http.CreateClient("core");
        var res = await client.PostAsJsonAsync(corePath, body, jsonOptions, ct);
        if (res.IsSuccessStatusCode)
        {
            var json = await res.Content.ReadAsStringAsync(ct);
            return Results.Content(json, "application/json");
        }
    }
    catch
    {
        // Core caído → fallback a outbox
    }

    db.Outbox.Add(new OutboxMessage
    {
        Type = outboxType,
        Payload = JsonSerializer.Serialize(body, jsonOptions),
        IdempotencyKey = Guid.NewGuid().ToString("N")
    });
    await db.SaveChangesAsync(ct);

    return Results.Accepted(uri: null, value: new { message = "Solicitud encolada. Se enviará a Core al restablecerse." });
}

async Task<IResult> TryProxyPatchAsync(
    string corePath,
    object body,
    IHttpClientFactory http,
    CancellationToken ct = default)
{
    var client = http.CreateClient("core");
    var req = new HttpRequestMessage(HttpMethod.Patch, corePath)
    {
        Content = new StringContent(JsonSerializer.Serialize(body, jsonOptions), Encoding.UTF8, "application/json")
    };

    var res = await client.SendAsync(req, ct);
    if (res.StatusCode == HttpStatusCode.NotFound)
        return Results.NotFound();

    res.EnsureSuccessStatusCode();
    var json = await res.Content.ReadAsStringAsync(ct);
    return Results.Content(json, "application/json");
}

// ---------------------- Endpoints públicos para Web/Caja ----------------------

// Health
app.MapGet("/health", () => Results.Ok(new { status = "UP" }));

var api = app.MapGroup("/api/v1");

// ===================== AUTH =====================
var auth = api.MapGroup("/auth");


// Login directo contra Core (sin outbox)
auth.MapPost("/login",
    async ([FromBody] LoginRequest dto, IHttpClientFactory http, CancellationToken ct) =>
    {
        var client = http.CreateClient("core");
        var res = await client.PostAsJsonAsync("/api/v1/auth/login", dto, jsonOptions, ct);
        if (!res.IsSuccessStatusCode) return Results.Unauthorized();
        return Results.Stream(await res.Content.ReadAsStreamAsync(ct), "application/json");
    });

// Registro de clientes (usa Outbox si Core cae)
auth.MapPost("/registro",
    async ([FromBody] RegistrarClienteRequest req, IntegracionDbContext db, IHttpClientFactory http, CancellationToken ct) =>
        await TryProxyPostAsync("/api/v1/auth/clientes", req, http, db, outboxType: "RegistrarCliente", ct));

// (Opcional) Cambiar password — directo
auth.MapPost("/cambiar-password",
    async ([FromBody] CambiarPasswordRequest req, IHttpClientFactory http, CancellationToken ct) =>
    {
        var client = http.CreateClient("core");
        var res = await client.PostAsJsonAsync("/api/v1/auth/cambiar-password", req, jsonOptions, ct);
        if (!res.IsSuccessStatusCode) return Results.BadRequest();
        return Results.Ok();
    });

// ===================== USUARIOS =====================
var usuarios = api.MapGroup("/usuarios");

// GET por id
usuarios.MapGet("/{id:int}",
    async (int id, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/usuarios/{id}", http, cache, cacheKey: $"user:{id}", ttl: TimeSpan.FromMinutes(5), ct));

// GET por email o todos
usuarios.MapGet("",
    async ([FromQuery] string? email, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
    {
        string corePath = string.IsNullOrWhiteSpace(email)
            ? "/api/v1/usuarios"
            : $"/api/v1/usuarios?email={Uri.EscapeDataString(email)}";

        string cacheKey = string.IsNullOrWhiteSpace(email) ? "users:all" : $"user:email:{email}";
        return await TryProxyGetAsync(corePath, http, cache, cacheKey, TimeSpan.FromMinutes(2), ct);
    });

// ===================== CONTRATOS =====================
var contratos = api.MapGroup("/contratos");

contratos.MapGet("/usuario/{usuarioId:int}",
    async (int usuarioId, [FromQuery] bool soloActivos, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/contratos/usuario/{usuarioId}?soloActivos={soloActivos}",
                               http, cache, cacheKey: $"contratos:{usuarioId}:{soloActivos}", ttl: TimeSpan.FromMinutes(1), ct));

// ===================== FACTURAS =====================
var facturas = api.MapGroup("/facturas");

facturas.MapGet("/pendientes/{usuarioId:int}",
    async (int usuarioId, IHttpClientFactory http, IntegracionDbContext db, CancellationToken ct) =>
    {
        try
        {
            // Primero Core
            var core = http.CreateClient("core");
            var res = await core.GetAsync($"/api/v1/facturas/pendientes/{usuarioId}", ct);
            if (res.IsSuccessStatusCode)
                return Results.Stream(await res.Content.ReadAsStreamAsync(ct), "application/json");
        }
        catch { /* Core caído */ }

        // Fallback: cache local (BD Integración)
        var cacheLocal = await db.Facturas
            .Where(f => f.UsuarioId == usuarioId && !f.EstaPagada)
            .ToListAsync(ct);

        return Results.Ok(cacheLocal);
    });

facturas.MapGet("/numero/{numero}",
    async (string numero, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/facturas/numero/{Uri.EscapeDataString(numero)}",
                               http, cache, cacheKey: $"factura:{numero}", ttl: TimeSpan.FromMinutes(1), ct));

facturas.MapGet("/usuario/{usuarioId:int}",
    async (int usuarioId, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/facturas/usuario/{usuarioId}",
                               http, cache, cacheKey: $"facturas:user:{usuarioId}", ttl: TimeSpan.FromMinutes(2), ct));

facturas.MapGet("/total-adeudado/{usuarioId:int}",
    async (int usuarioId, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/facturas/total-adeudado/{usuarioId}",
                               http, cache, cacheKey: $"facturas:total:{usuarioId}", ttl: TimeSpan.FromSeconds(30), ct));

// ===================== PAGOS =====================
var pagos = api.MapGroup("/pagos");

pagos.MapPost("",
    async ([FromBody] ProcesarPagoRequest body, IntegracionDbContext db, IHttpClientFactory http, CancellationToken ct) =>
        await TryProxyPostAsync("/api/v1/pagos", body, http, db, outboxType: "ProcesarPago", ct));

pagos.MapGet("/verificar/{numeroTransaccion}",
    async (string numeroTransaccion, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/pagos/verificar/{Uri.EscapeDataString(numeroTransaccion)}",
                               http, cache, cacheKey: $"pago:ver:{numeroTransaccion}", ttl: TimeSpan.FromSeconds(20), ct));

pagos.MapGet("/{pagoId:int}",
    async (int pagoId, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/pagos/{pagoId}",
                               http, cache, cacheKey: $"pago:{pagoId}", ttl: TimeSpan.FromMinutes(2), ct));

// ===================== AVERÍAS =====================
var averias = api.MapGroup("/averias");

averias.MapPost("",
    async ([FromBody] CrearAveriaRequest req, IHttpClientFactory http, IntegracionDbContext db, CancellationToken ct) =>
        await TryProxyPostAsync("/api/v1/averias", req, http, db, outboxType: "CrearAveria", ct));

averias.MapGet("/usuario/{usuarioId:int}",
    async (int usuarioId, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/averias/usuario/{usuarioId}",
                               http, cache, cacheKey: $"averias:user:{usuarioId}", ttl: TimeSpan.FromSeconds(30), ct));

averias.MapGet("/zona",
    async ([FromQuery] decimal latitud, [FromQuery] decimal longitud, [FromQuery] double radioKm,
           IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/averias/zona?latitud={latitud}&longitud={longitud}&radioKm={radioKm}",
                               http, cache, cacheKey: $"averias:zona:{latitud}:{longitud}:{radioKm}", ttl: TimeSpan.FromSeconds(20), ct));

averias.MapPatch("/{averiaId:int}/estado",
    async (int averiaId, [FromBody] ActualizarEstadoAveriaRequest req, IHttpClientFactory http, CancellationToken ct) =>
        await TryProxyPatchAsync($"/api/v1/averias/{averiaId}/estado", req, http, ct));

// ===================== NOTIFICACIONES =====================
var notis = api.MapGroup("/notificaciones");

notis.MapGet("/usuario/{usuarioId:int}",
    async (int usuarioId, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/notificaciones/usuario/{usuarioId}",
                               http, cache, cacheKey: $"notis:{usuarioId}", ttl: TimeSpan.FromSeconds(15), ct));

notis.MapPost("",
    async ([FromBody] EnviarNotificacionRequest req, IHttpClientFactory http, CancellationToken ct) =>
    {
        var client = http.CreateClient("core");
        var res = await client.PostAsJsonAsync("/api/v1/notificaciones", req, jsonOptions, ct);
        if (!res.IsSuccessStatusCode) return Results.BadRequest();
        return Results.Accepted();
    });

notis.MapPost("/{notificacionId:int}/leer",
    async (int notificacionId, IHttpClientFactory http, CancellationToken ct) =>
    {
        var client = http.CreateClient("core");
        var res = await client.PostAsync($"/api/v1/notificaciones/{notificacionId}/leer", null, ct);
        if (!res.IsSuccessStatusCode) return Results.BadRequest();
        return Results.NoContent();
    });

notis.MapGet("/usuario/{usuarioId:int}/no-leidas",
    async (int usuarioId, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/notificaciones/usuario/{usuarioId}/no-leidas",
                               http, cache, cacheKey: $"notis:nl:{usuarioId}", ttl: TimeSpan.FromSeconds(10), ct));

// ===================== SOLICITUDES =====================
var solicitudes = api.MapGroup("/solicitudes");

solicitudes.MapPost("",
    async ([FromBody] SolicitudCreateRequest sol, IHttpClientFactory http, CancellationToken ct) =>
    {
        var client = http.CreateClient("core");
        var res = await client.PostAsJsonAsync("/api/v1/solicitudes", sol, jsonOptions, ct);
        if (!res.IsSuccessStatusCode) return Results.BadRequest();
        return Results.Accepted();
    });

solicitudes.MapGet("/usuario/{usuarioId:int}",
    async (int usuarioId, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/solicitudes/usuario/{usuarioId}",
                               http, cache, cacheKey: $"sol:user:{usuarioId}", ttl: TimeSpan.FromMinutes(1), ct));

solicitudes.MapGet("/estado/{estado}",
    async (string estado, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/solicitudes/estado/{Uri.EscapeDataString(estado)}",
                               http, cache, cacheKey: $"sol:estado:{estado}", ttl: TimeSpan.FromMinutes(1), ct));

// ===================== ESTADO SERVICIO =====================
var estadoSrv = api.MapGroup("/estado-servicio");

estadoSrv.MapGet("/activos",
    async (IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync("/api/v1/estado-servicio/activos",
                               http, cache, cacheKey: $"estado:activos", ttl: TimeSpan.FromSeconds(20), ct));

estadoSrv.MapGet("/zona/{zona}",
    async (string zona, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/estado-servicio/zona/{Uri.EscapeDataString(zona)}",
                               http, cache, cacheKey: $"estado:zona:{zona}", ttl: TimeSpan.FromSeconds(20), ct));

estadoSrv.MapGet("/sector/{sector}",
    async (string sector, IHttpClientFactory http, IMemoryCache cache, CancellationToken ct) =>
        await TryProxyGetAsync($"/api/v1/estado-servicio/sector/{Uri.EscapeDataString(sector)}",
                               http, cache, cacheKey: $"estado:sector:{sector}", ttl: TimeSpan.FromSeconds(20), ct));

// ====== Webhook desde Core para upsert de administradores ======
api.MapPost("/webhooks/core/admin-upsert",
    async ([FromBody] UpsertAdminRequest req, HttpRequest httpReq, IConfiguration cfg, IntegracionDbContext db, CancellationToken ct) =>
    {
        if (!httpReq.Headers.TryGetValue("x-core-webhook", out var key) ||
            key != cfg["CoreWebhook:ApiKey"])
        {
            return Results.Unauthorized();
        }

        db.Outbox.Add(new OutboxMessage
        {
            Type = "UpsertWebAdmin",
            Payload = JsonSerializer.Serialize(req, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            IdempotencyKey = Guid.NewGuid().ToString("N")
        });

        await db.SaveChangesAsync(ct);
        return Results.Accepted();
    });

app.Run();


// ---------------------- DTOs para requests (LOCAL, sin dependencias a Core) ----------------------
public record LoginRequest(string Email, string Password);

// Usuario “ligero” para registro (debe mapear 1:1 con lo que Core espera en JSON)
public record UsuarioDto(
    string Cedula,
    string NombreCompleto,
    string Email,
    string Telefono,
    string Direccion,
    string? Rol // opcional; si no lo envías, Core asigna CLIENTE
);

public record RegistrarClienteRequest(UsuarioDto Usuario, string Password);

public record CambiarPasswordRequest(int UsuarioId, string PasswordActual, string PasswordNuevo);

public record ProcesarPagoRequest(int FacturaId, decimal Monto, string Metodo);

public record CrearAveriaRequest(
    int UsuarioId,
    string NumeroContrato,
    string Categoria,
    string Descripcion,
    string? Direccion,
    decimal? Latitud,
    decimal? Longitud,
    string? ContactoNombre,
    string? ContactoTelefono,
    string? ContactoEmail
);

public record ActualizarEstadoAveriaRequest(string NuevoEstado, string Comentario);

public record EnviarNotificacionRequest(int UsuarioId, string Titulo, string Mensaje, string? Tipo);

public record SolicitudCreateRequest(
    int UsuarioId,
    string Tipo,
    string Descripcion,
    string Estado
);
// DTO compartido para el webhook
public record UpsertAdminRequest(
    string Email,
    string? NombreCompleto,
    string? Telefono,
    string? TempPassword
);

// ---------------------- ResilienceHandler (reintentos + mini circuit breaker) ----------------------
public class ResilienceHandler : DelegatingHandler
{
    private static int _failCount = 0;
    private static DateTime _circuitOpenedUtc = DateTime.MinValue;
    private const int CircuitThreshold = 5;           // fallas para abrir
    private static readonly TimeSpan BreakDuration = TimeSpan.FromSeconds(30);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Circuit abierto?
        if (DateTime.UtcNow < _circuitOpenedUtc + BreakDuration)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                RequestMessage = request,
                ReasonPhrase = "Circuit open (custom)"
            };
        }

        int attempt = 0;
        Exception? lastEx = null;

        while (attempt < 3)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (IsTransientStatus(response.StatusCode))
                {
                    attempt++;
                    await Task.Delay(ComputeBackoff(attempt), cancellationToken);
                    continue;
                }

                // éxito → resetea circuito
                _failCount = 0;
                return response;
            }
            catch (HttpRequestException ex)
            {
                lastEx = ex;
                attempt++;
                await Task.Delay(ComputeBackoff(attempt), cancellationToken);
            }
            catch (TaskCanceledException ex) // timeouts
            {
                lastEx = ex;
                attempt++;
                await Task.Delay(ComputeBackoff(attempt), cancellationToken);
            }
        }

        // demasiadas fallas → abre circuito
        _failCount++;
        if (_failCount >= CircuitThreshold)
        {
            _circuitOpenedUtc = DateTime.UtcNow;
            _failCount = 0; // reinicia contador para la próxima ventana
        }

        if (lastEx != null) throw lastEx;
        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            RequestMessage = request,
            ReasonPhrase = "Retries exhausted (custom)"
        };
    }

    private static bool IsTransientStatus(HttpStatusCode code)
        => code == HttpStatusCode.RequestTimeout       // 408
        || code == (HttpStatusCode)429                 // TooManyRequests
        || ((int)code >= 500 && (int)code <= 599);     // 5xx

    private static TimeSpan ComputeBackoff(int attempt)
    {
        // 1: 200ms, 2: 400ms, 3: 800ms (cap a 2s)
        var ms = Math.Min(200 * (int)Math.Pow(2, attempt - 1), 2000);
        return TimeSpan.FromMilliseconds(ms);
    }
}
