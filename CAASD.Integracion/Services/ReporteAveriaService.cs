using System;
using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces;
using CAASD.Core.Interfaces.Services;
using CAASD.Core.Constants;

namespace CAASD.Integracion.Services
{
    public class ReporteAveriaService(IUnitOfWork unitOfWork) : IReporteAveriaService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public int CrearReporteAveria(ReporteAveria reporte)
        {
            try
            {
                reporte.FechaReporte = DateTime.Now;
                reporte.Estado = AppConstants.AVERIA_REPORTADA;

                if (string.IsNullOrEmpty(reporte.Prioridad))
                {
                    reporte.Prioridad = AppConstants.PRIORIDAD_MEDIA;
                }

                _unitOfWork.ReportesAveria.Add(reporte);
                _unitOfWork.SaveChanges();

                return reporte.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear reporte de avería: {ex.Message}");
            }
        }

        public IEnumerable<ReporteAveria> ObtenerAveriasPorUsuario(int usuarioId)
        {
            try
            {
                return _unitOfWork.ReportesAveria.GetAveriasByUsuario(usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener averías del usuario: {ex.Message}");
            }
        }

        public IEnumerable<ReporteAveria> ObtenerAveriasEnZona(decimal latitud, decimal longitud, double radioKm)
        {
            try
            {
                return _unitOfWork.ReportesAveria.GetAveriasByZona(latitud, longitud, radioKm);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener averías en la zona: {ex.Message}");
            }
        }

        public bool ActualizarEstadoAveria(int averiaId, string nuevoEstado, string comentario)
        {
            try
            {
                var averia = _unitOfWork.ReportesAveria.GetById(averiaId);

                if (averia == null)
                {
                    throw new Exception("Avería no encontrada");
                }

                averia.Estado = nuevoEstado;
                averia.ComentarioResolucion = comentario;

                if (nuevoEstado == AppConstants.AVERIA_RESUELTA ||
                    nuevoEstado == AppConstants.AVERIA_CERRADA)
                {
                    averia.FechaResolucion = DateTime.Now;
                }

                _unitOfWork.ReportesAveria.Update(averia);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar estado de avería: {ex.Message}");
            }
        }
    }
}