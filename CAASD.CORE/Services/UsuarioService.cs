using System;
using System.Collections.Generic;
using CAASD.Core.Entities;
using CAASD.Core.Helpers;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Core.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    public UsuarioService(IUsuarioRepository repo) => _repo = repo;

    public IEnumerable<Usuario> ObtenerTodos() => _repo.GetAll();
    public Usuario? ObtenerPorId(int id) => _repo.GetById(id);
    public Usuario? ObtenerPorEmail(string email) => _repo.GetByEmail(email);

    public void Crear(Usuario usuario, string passwordPlano)
    {
        usuario.PasswordHash = SecurityHelper.HashPassword(passwordPlano);
        usuario.Activo = true;
        usuario.FechaRegistro = DateTime.Now;
        _repo.Add(usuario);
    }

    public void Actualizar(Usuario usuario) => _repo.Update(usuario);

    public bool CambiarRol(int usuarioId, int nuevoRolId)
    {
        var u = _repo.GetById(usuarioId);
        if (u == null) return false;
        if (u.Id == 9999) return false; // protege ROOT
        u.RolId = nuevoRolId;
        _repo.Update(u);
        return true;
    }
}
