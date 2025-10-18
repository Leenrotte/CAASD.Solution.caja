using Microsoft.EntityFrameworkCore;

namespace CAASD.Integracion.Infrastructure;

public class IntegracionDbContext : DbContext
{
    public IntegracionDbContext(DbContextOptions<IntegracionDbContext> opt) : base(opt) { }

    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    public DbSet<UsuarioCache> Usuarios => Set<UsuarioCache>();
    public DbSet<FacturaCache> Facturas => Set<FacturaCache>();
    public DbSet<PagoCache> Pagos => Set<PagoCache>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<OutboxMessage>().HasKey(x => x.Id);
        mb.Entity<OutboxMessage>().Property(x => x.Type).HasMaxLength(100).IsRequired();
        mb.Entity<OutboxMessage>().Property(x => x.Status).HasMaxLength(30).HasDefaultValue("Pending");
        mb.Entity<OutboxMessage>().HasIndex(x => new { x.Type, x.IdempotencyKey }).IsUnique();

        mb.Entity<UsuarioCache>().HasIndex(x => x.Email).IsUnique();

        mb.Entity<FacturaCache>().HasIndex(x => x.NumeroFactura).IsUnique();
        // FacturaCache: decimales
        mb.Entity<FacturaCache>()
          .Property(x => x.MontoTotal)
          .HasPrecision(18, 2);

        // PagoCache: decimales
        mb.Entity<PagoCache>()
          .Property(x => x.Monto)
          .HasPrecision(18, 2);

        // (opcional) tamaños de strings comunes
        mb.Entity<PagoCache>().Property(x => x.NumeroTransaccion).HasMaxLength(100);
        mb.Entity<PagoCache>().Property(x => x.Metodo).HasMaxLength(50);
        mb.Entity<FacturaCache>().Property(x => x.NumeroFactura).HasMaxLength(50);

    }
}
