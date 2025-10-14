using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class ReporteAveriaRepository(CAASDContext context) : Repository<ReporteAveria>(context), IReporteAveriaRepository
    {
        public IEnumerable<ReporteAveria> GetAveriasByUsuario(int usuarioId)
        {
            return [.. _dbSet
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaReporte)];
        }

        public IEnumerable<ReporteAveria> GetAveriasByEstado(string estado)
        {
            return [.. _dbSet
                .Where(r => r.Estado == estado)
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaReporte)];
        }

        public IEnumerable<ReporteAveria> GetAveriasByZona(decimal latitud, decimal longitud, double radioKm)
        {
            var averias = _dbSet
                .Include(r => r.Usuario)
                .ToList()
                .Where(r =>
                {
                    double distance = CalcularDistancia(
                        (double)latitud,
                        (double)longitud,
                        (double)r.Latitud,
                        (double)r.Longitud
                    );
                    return distance <= radioKm;
                })
                .OrderByDescending(r => r.FechaReporte)
                .ToList();

            return averias;
        }

        private static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}