using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Entities;


namespace CAASD.Core.Interfaces.Services
{
    public interface IReporteAveriaService
    {
        int CrearReporteAveria(ReporteAveria reporte);
        IEnumerable<ReporteAveria> ObtenerAveriasPorUsuario(int usuarioId);
        IEnumerable<ReporteAveria> ObtenerAveriasEnZona(decimal latitud, decimal longitud, double radioKm);
        bool ActualizarEstadoAveria(int averiaId, string nuevoEstado, string comentario);
    }
}