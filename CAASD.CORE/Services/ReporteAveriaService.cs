using System.Collections.Generic;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Core.Services;

public class ReporteAveriaService : IReporteAveriaService
{
    private readonly IReporteAveriaRepository _repo;
    public ReporteAveriaService(IReporteAveriaRepository repo) => _repo = repo;

    public IEnumerable<ReporteAveria> ObtenerAveriasEnZona(decimal lat, decimal lon, double radioKm)
        => _repo.GetAveriasByZona(lat, lon, radioKm);

    public IEnumerable<ReporteAveria> ObtenerAveriasPorUsuario(int usuarioId)
        => _repo.GetAveriasByUsuario(usuarioId);

    public bool ActualizarEstadoAveria(int averiaId, string nuevoEstado, string comentario)
    {
        var a = _repo.GetById(averiaId);
        if (a == null) return false;
        a.Estado = nuevoEstado;
        a.ComentarioResolucion = comentario;
        if (nuevoEstado == "Resuelta" || nuevoEstado == "Cerrada")
            a.FechaResolucion = System.DateTime.Now;
        _repo.Update(a);
        return true;
    }

    public int CrearReporteAveria(ReporteAveria reporte)
    {
        _repo.Add(reporte);
        return reporte.Id;
    }
}
