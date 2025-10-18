// CAASD.CORE/Infrastructure/CaasdCoreDbContextFactory.cs
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CAASD.Core.Infrastructure
{
    public class CaasdCoreDbContextFactory : IDesignTimeDbContextFactory<CaasdCoreDbContext>
    {
        public CaasdCoreDbContext CreateDbContext(string[] args)
        {
            // BasePath = carpeta del proyecto cuando usas -Project en los comandos EF
            var basePath = Directory.GetCurrentDirectory();

            var cfg = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Lee la cadena de conexión "CAASDConnection"
            var conn = cfg.GetConnectionString("CAASDConnection")
                       ?? "Server=AyleenFigueroa\\SQLEXPRESS;Database=CAASD_CoreDB;Trusted_Connection=True;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<CaasdCoreDbContext>()
                .UseSqlServer(conn)
                .Options;

            return new CaasdCoreDbContext(options);
        }
    }
}
