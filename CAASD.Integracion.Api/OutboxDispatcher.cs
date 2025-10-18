using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CAASD.Integracion.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class OutboxDispatcher : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<OutboxDispatcher> _log;


    public OutboxDispatcher(IServiceProvider sp, ILogger<OutboxDispatcher> log)
    {
        _sp = sp;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("OutboxDispatcher iniciado...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IntegracionDbContext>();

                // Obtenemos el factory para crear los dos clientes (Core y Web)
                var factory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                var httpCore = factory.CreateClient("core");
                var httpWeb = factory.CreateClient("web");


                var pend = await db.Outbox
                    .Where(x => x.Status == "Pending")
                    .OrderBy(x => x.CreatedAt)
                    .Take(25)
                    .ToListAsync(stoppingToken);

                if (pend.Count == 0)
                {
                    await Task.Delay(3000, stoppingToken);
                    continue;
                }

                foreach (var msg in pend)
                {
                    try
                    {
                        using var content = new StringContent(msg.Payload, Encoding.UTF8, "application/json");
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        content.Headers.Add("Idempotency-Key", msg.IdempotencyKey);

                        HttpResponseMessage res = msg.Type switch
                        {
                            "RegistrarCliente" => await httpCore.PostAsync("/api/v1/auth/clientes", content, stoppingToken),
                            "ProcesarPago" => await httpCore.PostAsync("/api/v1/pagos", content, stoppingToken),
                            "CrearAveria" => await httpCore.PostAsync("/api/v1/averias", content, stoppingToken),

                            // Nuevo caso para replicar administradores hacia Web
                            "UpsertWebAdmin" => await httpWeb.PostAsync("/internal/admins/upsert", content, stoppingToken),

                            _ => new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                        };


                        if (res.IsSuccessStatusCode)
                        {
                            msg.Status = "Sent";
                            msg.SentAt = DateTime.UtcNow;
                            msg.LastError = null;
                            _log.LogInformation(" Outbox {Type} reenviado con éxito (Id {Id})", msg.Type, msg.Id);
                        }
                        else
                        {
                            msg.Retries++;
                            msg.LastError = $"Error {res.StatusCode}";
                            _log.LogWarning("Outbox fallo HTTP {StatusCode} ({Type} Id {Id})", res.StatusCode, msg.Type, msg.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        msg.Retries++;
                        msg.LastError = ex.Message;
                        _log.LogError(ex, "Error reenviando mensaje {Type} Id {Id}", msg.Type, msg.Id);
                    }

                    if (msg.Retries > 10)
                    {
                        msg.Status = "Failed";
                        _log.LogError("Mensaje {Id} marcado como Failed tras {Retries} intentos", msg.Id, msg.Retries);
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, " Error global en OutboxDispatcher.");
            }

            // espera incremental (no saturar)
            await Task.Delay(2000, stoppingToken);
        }

        _log.LogWarning("OutboxDispatcher detenido.");
    }
}
