namespace CAASD.Integracion;

public class OutboxMessage
{
    public long Id { get; set; }
    public string Type { get; set; } = default!;         // "RegistrarCliente", "ProcesarPago"
    public string Payload { get; set; } = default!;      // JSON del comando
    public string IdempotencyKey { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public int Retries { get; set; }
    public string Status { get; set; } = "Pending";      // Pending|Sent|Failed
    public string? LastError { get; set; }
}
