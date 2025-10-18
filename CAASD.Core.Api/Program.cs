using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using CAASD.Core.Infrastructure;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;

// ===== CREACIÓN DEL BUILDER =====
var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<CaasdCoreDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("CAASDConnection")));

// Repositorios (SOLO los que existen en tu Core)
builder.Services.AddScoped<IUsuarioRepository, CAASD.Core.Repositories.UsuarioRepository>();
builder.Services.AddScoped<IContratoRepository, CAASD.Core.Repositories.ContratoRepository>();
builder.Services.AddScoped<IFacturaRepository, CAASD.Core.Repositories.FacturaRepository>();
builder.Services.AddScoped<IPagoRepository, CAASD.Core.Repositories.PagoRepository>();
builder.Services.AddScoped<IReporteAveriaRepository, CAASD.Core.Repositories.ReporteAveriaRepository>();
builder.Services.AddScoped<INotificacionRepository, CAASD.Core.Repositories.NotificacionRepository>();
builder.Services.AddScoped<IEstadoServicioRepository, CAASD.Core.Repositories.EstadoServicioRepository>();
builder.Services.AddScoped<ISolicitudRepository, CAASD.Core.Repositories.SolicitudRepository>();

// Servicios (tus implementaciones en CAASD.Core.Services)
builder.Services.AddScoped<IAuthService, CAASD.Core.Services.AuthService>();
builder.Services.AddScoped<IUsuarioService, CAASD.Core.Services.UsuarioService>();
builder.Services.AddScoped<IContratoService, CAASD.Core.Services.ContratoService>();
builder.Services.AddScoped<IFacturaService, CAASD.Core.Services.FacturaService>();
builder.Services.AddScoped<IPagoService, CAASD.Core.Services.PagoService>();
builder.Services.AddScoped<IReporteAveriaService, CAASD.Core.Services.ReporteAveriaService>();
builder.Services.AddScoped<INotificacionService, CAASD.Core.Services.NotificacionService>();

// CORS (para pruebas locales de Integración)
builder.Services.AddCors(o => o.AddPolicy("AllowLocal",
    p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// JWT opcional
var jwtKey = builder.Configuration["Auth:JwtKey"] ?? "dev-key-change-me";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// API-Key para Integración
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IntegrationOnly", policy =>
        policy.RequireAssertion(ctx =>
        {
            if (ctx.Resource is HttpContext http &&
                http.Request.Headers.TryGetValue("x-api-key", out StringValues key))
            {
                var expected = http.RequestServices.GetRequiredService<IConfiguration>()["Integration:ApiKey"] ?? "dev-key";
                return key == expected;
            }
            return false;
        }));
});

// Swagger + seguridad (JWT + API Key)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CAASD.Core.Api", Version = "v1" });

    // JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer. Ej: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // API Key
    c.AddSecurityDefinition("XApiKey", new OpenApiSecurityScheme
    {
        Description = "Api Key para Integración",
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "XApiKey" } },
            Array.Empty<string>()
        }
    });
});

// Logging consola
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o =>
{
    o.SingleLine = true;
    o.TimestampFormat = "HH:mm:ss ";
});

var app = builder.Build();

app.UseCors("AllowLocal");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CAASD.Core.Api v1");
});

app.MapGet("/", () => Results.Ok(new { service = "CAASD.Core.Api", status = "OK" }));

// ====== VERSIONAMIENTO Y GRUPOS ======
var api = app.MapGroup("/api/v1");

// ---------- AUTH ----------
var auth = api.MapGroup("/auth");
auth.MapPost("/login",
    ([FromBody] CAASD.Core.DTOs.LoginRequestDTO dto, IAuthService authSvc) =>
    {
        var res = authSvc.Login(dto);
        return res.Success ? Results.Ok(res) : Results.Unauthorized();
    })
.WithName("Auth_Login");

// Crear STAFF (ADMINISTRADOR/CAJERO) vía Integración
auth.MapPost("/staff",
    [Authorize(Policy = "IntegrationOnly")]
([FromBody] CrearStaffRequest req, IAuthService authSvc) =>
    {
        var ok = authSvc.RegistrarUsuario(req.Usuario, req.Password);
        return ok ? Results.Ok() : Results.Conflict("No se pudo crear (posible duplicado).");
    })
.RequireAuthorization("IntegrationOnly")
.WithName("Auth_CrearStaff");

// Registrar CLIENTE (Integración/Web)
auth.MapPost("/clientes",
    [Authorize(Policy = "IntegrationOnly")]
([FromBody] RegistrarClienteRequest req, IAuthService authSvc) =>
    {
        // Internamente el servicio forzará el rol CLIENTE
        var ok = authSvc.RegistrarUsuario(req.Usuario, req.Password);
        return ok ? Results.Ok() : Results.Conflict("No se pudo crear (posible duplicado).");
    })
.RequireAuthorization("IntegrationOnly")
.WithName("Auth_RegistrarCliente");

// Cambiar password (JWT o x-api-key según tu escenario)
auth.MapPost("/cambiar-password",
    ([FromBody] CambiarPasswordRequest req, IAuthService authSvc) =>
    {
        var ok = authSvc.CambiarPassword(req.UsuarioId, req.PasswordActual, req.PasswordNuevo);
        return ok ? Results.Ok() : Results.BadRequest("No se pudo cambiar la contraseña");
    })
.WithName("Auth_CambiarPassword");

// ---------- USUARIOS ----------
var usuarios = api.MapGroup("/usuarios").RequireAuthorization("IntegrationOnly");

usuarios.MapGet("/{id:int}", (int id, IUsuarioService svc) =>
{
    var u = svc.ObtenerPorId(id);
    return u is null ? Results.NotFound() : Results.Ok(u);
}).WithName("Usuarios_GetById");

usuarios.MapGet("", ([FromQuery] string? email, IUsuarioService svc) =>
{
    if (!string.IsNullOrWhiteSpace(email))
    {
        var u = svc.ObtenerPorEmail(email);
        return u is null ? Results.NotFound() : Results.Ok(u);
    }
    return Results.Ok(svc.ObtenerTodos());
}).WithName("Usuarios_Query");

// ---------- CONTRATOS ----------
var contratos = api.MapGroup("/contratos").RequireAuthorization("IntegrationOnly");

contratos.MapGet("/usuario/{usuarioId:int}",
    (int usuarioId, IContratoService svc, [FromQuery] bool soloActivos) =>
    {
        return Results.Ok(svc.ObtenerPorUsuario(usuarioId, soloActivos));
    }).WithName("Contratos_PorUsuario");

// ---------- FACTURAS ----------
var facturas = api.MapGroup("/facturas").RequireAuthorization("IntegrationOnly");

facturas.MapGet("/pendientes/{usuarioId:int}",
    (int usuarioId, IFacturaService svc) =>
    {
        return Results.Ok(svc.ObtenerFacturasPendientes(usuarioId));
    }).WithName("Facturas_Pendientes");

facturas.MapGet("/numero/{numero}",
    (string numero, IFacturaService svc) =>
    {
        var fx = svc.ObtenerFacturaPorNumero(numero);
        return fx is null ? Results.NotFound() : Results.Ok(fx);
    }).WithName("Facturas_PorNumero");

facturas.MapGet("/usuario/{usuarioId:int}",
    (int usuarioId, IFacturaService svc) =>
    {
        return Results.Ok(svc.ObtenerFacturasPorUsuario(usuarioId));
    }).WithName("Facturas_PorUsuario");

facturas.MapGet("/total-adeudado/{usuarioId:int}",
    (int usuarioId, IFacturaService svc) =>
    {
        return Results.Ok(new { usuarioId, total = svc.ObtenerTotalAdeudado(usuarioId) });
    }).WithName("Facturas_TotalAdeudado");

// ---------- PAGOS ----------
var pagos = api.MapGroup("/pagos").RequireAuthorization("IntegrationOnly");

pagos.MapPost("",
    ([FromBody] ProcesarPagoRequest body, IPagoService svc) =>
    {
        var ok = svc.ProcesarPago(body.FacturaId, body.Monto, body.Metodo);
        return ok ? Results.Ok() : Results.BadRequest("No se pudo procesar el pago");
    }).WithName("Pagos_Procesar");

pagos.MapGet("/verificar/{numeroTransaccion}",
    (string numeroTransaccion, IPagoService svc) =>
    {
        return Results.Ok(new { numeroTransaccion, existe = svc.VerificarPago(numeroTransaccion) });
    }).WithName("Pagos_Verificar");

pagos.MapGet("/{pagoId:int}",
    (int pagoId, IPagoService svc) =>
    {
        var dto = svc.ObtenerDetallePago(pagoId);
        return dto is null ? Results.NotFound() : Results.Ok(dto);
    }).WithName("Pagos_Get");

// ---------- AVERÍAS ----------
var averias = api.MapGroup("/averias").RequireAuthorization("IntegrationOnly");

averias.MapPost("",
    ([FromBody] ReporteAveria reporte, IReporteAveriaService svc) =>
    {
        var id = svc.CrearReporteAveria(reporte);
        return Results.Created($"/api/v1/averias/{id}", new { id });
    }).WithName("Averias_Crear");

averias.MapGet("/usuario/{usuarioId:int}",
    (int usuarioId, IReporteAveriaService svc) =>
    {
        return Results.Ok(svc.ObtenerAveriasPorUsuario(usuarioId));
    }).WithName("Averias_PorUsuario");

averias.MapGet("/zona",
    ([FromQuery] decimal latitud, [FromQuery] decimal longitud, [FromQuery] double radioKm,
     IReporteAveriaService svc) =>
    {
        return Results.Ok(svc.ObtenerAveriasEnZona(latitud, longitud, radioKm));
    }).WithName("Averias_PorZona");

averias.MapPatch("/{averiaId:int}/estado",
    (int averiaId, [FromBody] ActualizarEstadoAveriaRequest req, IReporteAveriaService svc) =>
    {
        var ok = svc.ActualizarEstadoAveria(averiaId, req.NuevoEstado, req.Comentario);
        return ok ? Results.NoContent() : Results.BadRequest("No se pudo actualizar el estado");
    }).WithName("Averias_ActualizarEstado");

// ---------- NOTIFICACIONES ----------
var notis = api.MapGroup("/notificaciones").RequireAuthorization("IntegrationOnly");

notis.MapGet("/usuario/{usuarioId:int}",
    (int usuarioId, INotificacionService svc) =>
    {
        return Results.Ok(svc.ObtenerNotificacionesUsuario(usuarioId));
    }).WithName("Notis_PorUsuario");

notis.MapPost("",
    ([FromBody] EnviarNotificacionRequest req, INotificacionService svc) =>
    {
        svc.EnviarNotificacion(req.UsuarioId, req.Titulo, req.Mensaje, req.Tipo ?? "general");
        return Results.Accepted();
    }).WithName("Notis_Enviar");

notis.MapPost("/{notificacionId:int}/leer",
    (int notificacionId, INotificacionService svc) =>
    {
        svc.MarcarComoLeida(notificacionId);
        return Results.NoContent();
    }).WithName("Notis_MarcarLeida");

notis.MapGet("/usuario/{usuarioId:int}/no-leidas",
    (int usuarioId, INotificacionService svc) =>
    {
        return Results.Ok(new { usuarioId, noLeidas = svc.ContarNotificacionesNoLeidas(usuarioId) });
    }).WithName("Notis_ContarNoLeidas");

// ---------- SOLICITUDES ----------
var solicitudes = api.MapGroup("/solicitudes").RequireAuthorization("IntegrationOnly");

solicitudes.MapPost("",
    ([FromBody] Solicitud sol, ISolicitudRepository repo) =>
    {
        repo.Add(sol);
        return Results.Created($"/api/v1/solicitudes/{sol.Id}", new { id = sol.Id });
    }).WithName("Solicitudes_Crear");

solicitudes.MapGet("/usuario/{usuarioId:int}",
    (int usuarioId, ISolicitudRepository repo) =>
    {
        return Results.Ok(repo.Find(s => s.UsuarioId == usuarioId));
    }).WithName("Solicitudes_PorUsuario");

solicitudes.MapGet("/estado/{estado}",
    (string estado, ISolicitudRepository repo) =>
    {
        return Results.Ok(repo.Find(s => s.Estado == estado));
    }).WithName("Solicitudes_PorEstado");

// ---------- ESTADO DE SERVICIO ----------
var estado = api.MapGroup("/estado-servicio").RequireAuthorization("IntegrationOnly");

estado.MapGet("/activos",
    (IEstadoServicioRepository repo) => Results.Ok(repo.GetActivos()))
    .WithName("EstadoServicio_Activos");

estado.MapGet("/zona/{zona}",
    (string zona, IEstadoServicioRepository repo) => Results.Ok(repo.GetByZona(zona)))
    .WithName("EstadoServicio_PorZona");

estado.MapGet("/sector/{sector}",
    (string sector, IEstadoServicioRepository repo) => Results.Ok(repo.GetBySector(sector)))
    .WithName("EstadoServicio_PorSector");

app.Run();

// ======= DTOs de requests locales (para Minimal APIs) =======
public record CrearStaffRequest(CAASD.Core.DTOs.UsuarioDTO Usuario, string Password);
public record RegistrarClienteRequest(CAASD.Core.DTOs.UsuarioDTO Usuario, string Password);
public record CambiarPasswordRequest(int UsuarioId, string PasswordActual, string PasswordNuevo);
public record ProcesarPagoRequest(int FacturaId, decimal Monto, string Metodo);
public record EnviarNotificacionRequest(int UsuarioId, string Titulo, string Mensaje, string? Tipo);
public record ActualizarEstadoAveriaRequest(string NuevoEstado, string Comentario);
