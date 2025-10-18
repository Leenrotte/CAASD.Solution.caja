using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Core.Services;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _repo;
    public NotificacionService(INotificacionRepository repo) => _repo = repo;

    public void EnviarNotificacion(int usuarioId, string titulo, string mensaje, string tipo)
        => _repo.Add(new Notificacion { UsuarioId = usuarioId, Titulo = titulo, Mensaje = mensaje, Tipo = tipo, FechaEnvio = System.DateTime.Now, Leida = false });

    public IEnumerable<Notificacion> ObtenerNotificacionesUsuario(int usuarioId)
        => _repo.GetByUsuario(usuarioId);

    public void MarcarComoLeida(int notificacionId)
    {
        var n = _repo.GetById(notificacionId);
        if (n == null) return;
        n.Leida = true;
        _repo.Update(n);
    }

    public int ContarNotificacionesNoLeidas(int usuarioId)
        => _repo.GetByUsuario(usuarioId).Count(n => !n.Leida);
}
