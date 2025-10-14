using System;
using System.Collections.Generic;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Integracion.Services
{
    public class NotificacionService(IUnitOfWork unitOfWork) : INotificacionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public void EnviarNotificacion(int usuarioId, string titulo, string mensaje, string tipo)
        {
            try
            {
                var notificacion = new Notificacion
                {
                    UsuarioId = usuarioId,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    Tipo = tipo,
                    FechaEnvio = DateTime.Now,
                    Leida = false,
                    Url = string.Empty
                };

                _unitOfWork.Notificaciones.Add(notificacion);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar notificación: {ex.Message}");
            }
        }

        public IEnumerable<Notificacion> ObtenerNotificacionesUsuario(int usuarioId)
        {
            try
            {
                return _unitOfWork.Notificaciones.GetByUsuario(usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener notificaciones: {ex.Message}");
            }
        }

        public void MarcarComoLeida(int notificacionId)
        {
            try
            {
                var notificacion = _unitOfWork.Notificaciones.GetById(notificacionId);

                if (notificacion == null)
                    return;

                notificacion.Leida = true;
                _unitOfWork.Notificaciones.Update(notificacion);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al marcar notificación como leída: {ex.Message}");
            }
        }

        public int ContarNotificacionesNoLeidas(int usuarioId)
        {
            try
            {
                return _unitOfWork.Notificaciones.ContarNoLeidasByUsuario(usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar notificaciones no leídas: {ex.Message}");
            }
        }
    }
}