using System;
using System.Collections.Generic;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces;
using CAASD.Core.Constants;

namespace CAASD.Integracion.Services
{
    public class SolicitudService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public int CrearSolicitud(Solicitud solicitud)
        {
            try
            {
                solicitud.FechaSolicitud = DateTime.Now;
                solicitud.Estado = AppConstants.ESTADO_PENDIENTE;
                solicitud.Respuesta = string.Empty;
                solicitud.DocumentosAdjuntos = string.Empty;

                _unitOfWork.Solicitudes.Add(solicitud);
                _unitOfWork.SaveChanges();

                return solicitud.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear solicitud: {ex.Message}");
            }
        }

        public IEnumerable<Solicitud> ObtenerSolicitudesPorUsuario(int usuarioId)
        {
            try
            {
                return _unitOfWork.Solicitudes.GetByUsuario(usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener solicitudes: {ex.Message}");
            }
        }

        public IEnumerable<Solicitud> ObtenerSolicitudesPendientes()
        {
            try
            {
                return _unitOfWork.Solicitudes.GetPendientes();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener solicitudes pendientes: {ex.Message}");
            }
        }

        public bool ProcesarSolicitud(int solicitudId, string respuesta)
        {
            try
            {
                var solicitud = _unitOfWork.Solicitudes.GetById(solicitudId);

                if (solicitud == null)
                    return false;

                solicitud.Estado = AppConstants.ESTADO_PROCESADO;
                solicitud.Respuesta = respuesta;
                solicitud.FechaRespuesta = DateTime.Now;

                _unitOfWork.Solicitudes.Update(solicitud);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar solicitud: {ex.Message}");
            }
        }
    }
}