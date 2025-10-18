using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;

namespace CAASD.Core.Infrastructure;

public class CaasdCoreDbContext : DbContext
{
    public CaasdCoreDbContext(DbContextOptions<CaasdCoreDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<ReporteAveria> ReportesAveria => Set<ReporteAveria>();
    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<ContenidoEducativo> ContenidosEducativos => Set<ContenidoEducativo>();
    public DbSet<EstadoServicio> EstadosServicio => Set<EstadoServicio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
        });
        // Seed de roles iniciales
        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = 1, Nombre = "Superadmin", Descripcion = "Rol con todos los permisos" },
            new Rol { Id = 2, Nombre = "Administrador", Descripcion = "Rol para gestión administrativa" },
            new Rol { Id = 3, Nombre = "Cliente", Descripcion = "Usuario cliente del sistema" },
            new Rol { Id = 4, Nombre = "Cajero", Descripcion = "Rol encargado de pagos y cobros" }
        );

        modelBuilder.Entity<Usuario>().HasData(
           new Usuario
           {
               Id = 9999,
               Cedula = "00000000000",
               Nombre = "Root",
               Apellido = "CAASD",
               Email = "superadmin@caasd.gob.do",
               Telefono = "809-000-0000",
               DireccionCompleta = "Sede CAASD",
               PasswordHash = "c8ca302fe651ad24cbfd711fd4b5e37533660cab1764b454cc4182c76b94224c", // SHA-256(Caasd#2025!)
               Activo = true,
               RolId = 1,
               // Usa una fecha fija (determinística) para semillas EF Core:
               FechaRegistro = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)

           }
         );

        modelBuilder.Entity<Usuario>()
          .Navigation(u => u.Rol)
          .AutoInclude();

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.DireccionCompleta).HasMaxLength(500);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);

            entity.HasOne(e => e.Rol)
                .WithMany()
                .HasForeignKey(e => e.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Cedula).IsUnique();
        });

        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroContrato).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Direccion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Sector).HasMaxLength(100);
            entity.Property(e => e.Latitud).HasPrecision(10, 8);
            entity.Property(e => e.Longitud).HasPrecision(11, 8);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.NumeroContrato).IsUnique();
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroFactura).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MontoAgua).HasPrecision(18, 2);
            entity.Property(e => e.MontoAlcantarillado).HasPrecision(18, 2);
            entity.Property(e => e.MontoBasura).HasPrecision(18, 2);
            entity.Property(e => e.Itbis).HasPrecision(18, 2);
            entity.Property(e => e.MontoTotal).HasPrecision(18, 2);
            entity.Property(e => e.EstadoPago).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Contrato)
                .WithMany()
                .HasForeignKey(e => e.ContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.NumeroFactura).IsUnique();
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasPrecision(18, 2);
            entity.Property(e => e.MetodoPago).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NumeroTransaccion).HasMaxLength(100);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ReferenciaExterna).HasMaxLength(100);

            entity.HasOne(e => e.Factura)
                .WithMany()
                .HasForeignKey(e => e.FacturaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.NumeroTransaccion).IsUnique();
        });

        modelBuilder.Entity<ReporteAveria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoAveria).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Direccion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Latitud).HasPrecision(10, 8);
            entity.Property(e => e.Longitud).HasPrecision(11, 8);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Prioridad).HasMaxLength(50);
            entity.Property(e => e.FotoUrl).HasMaxLength(500);
            entity.Property(e => e.ComentarioResolucion).HasMaxLength(1000);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoSolicitud).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Respuesta).HasMaxLength(1000);
            entity.Property(e => e.DocumentosAdjuntos).HasMaxLength(500);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Mensaje).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ContenidoEducativo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Categoria).HasMaxLength(100);
            entity.Property(e => e.ImagenUrl).HasMaxLength(500);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<EstadoServicio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Zona).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Sector).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.TipoInterrupcion).HasMaxLength(100);
        });
    }
}

