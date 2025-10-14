using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Entities;


namespace CAASD.Core.Interfaces.Services
{
    public interface INotificacionService
    {
        void EnviarNotificacion(int usuarioId, string titulo, string mensaje, string tipo);
        IEnumerable<Notificacion> ObtenerNotificacionesUsuario(int usuarioId);
        void MarcarComoLeida(int notificacionId);
        int ContarNotificacionesNoLeidas(int usuarioId);
    }
}