using System;
using CAASD.Core.Constants;
using CAASD.Core.DTOs;
using CAASD.Core.Entities;
using CAASD.Core.Helpers;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    public AuthService(IUsuarioRepository usuarios) => _usuarios = usuarios;

    public LoginResponseDTO Login(LoginRequestDTO dto)
    {
        var user = _usuarios.GetByEmail(dto.Email);
        var ok = user != null && SecurityHelper.VerifyPassword(dto.Password, user.PasswordHash);

        return new LoginResponseDTO
        {
            Success = ok && user!.Activo,
            Mensaje = ok ? "OK" : "Credenciales inválidas",
            Usuario = ok ? new UsuarioDTO
            {
                Id = user!.Id,
                Cedula = user.Cedula,
                NombreCompleto = $"{user.Nombre} {user.Apellido}",
                Email = user.Email,
                Direccion = user.DireccionCompleta ?? "",
                Telefono = user.Telefono ?? "",
                Rol = user.Rol?.Nombre ?? AppConstants.ROL_CLIENTE,
                EsAdministrador = (user.Rol?.Nombre?.Equals(AppConstants.ROL_SUPERADMIN, StringComparison.OrdinalIgnoreCase) ?? false)
                                  || user.RolId == AppConstants.ROLID_SUPERADMIN
            } : null
        };
    }

    public bool RegistrarUsuario(UsuarioDTO usuario, string password)
    {
        if (_usuarios.GetByEmail(usuario.Email) != null) return false;
        var (nombre, apellido) = SplitNombre(usuario.NombreCompleto);

        var u = new Usuario
        {
            Cedula = usuario.Cedula,
            Nombre = nombre,
            Apellido = apellido,
            Email = usuario.Email,
            DireccionCompleta = usuario.Direccion,
            Telefono = usuario.Telefono,
            RolId = ResolveRolId(usuario.Rol, usuario.EsAdministrador),
            Activo = true,
            FechaRegistro = DateTime.Now,
            PasswordHash = SecurityHelper.HashPassword(password)
        };
        _usuarios.Add(u);
        return true;
    }

    public bool CambiarPassword(int usuarioId, string passwordActual, string passwordNuevo)
    {
        var u = _usuarios.GetById(usuarioId);
        if (u == null) return false;
        if (!SecurityHelper.VerifyPassword(passwordActual, u.PasswordHash)) return false;
        u.PasswordHash = SecurityHelper.HashPassword(passwordNuevo);
        _usuarios.Update(u);
        return true;
    }

    public bool RecuperarPassword(string email) => _usuarios.GetByEmail(email) != null;

    public bool RegistrarClienteDesdeWeb(UsuarioDTO dto, string password)
    {
        dto.Rol = AppConstants.ROL_CLIENTE;
        dto.EsAdministrador = false;
        return RegistrarUsuario(dto, password);
    }

    private static (string nombre, string apellido) SplitNombre(string nombreCompleto)
    {
        var n = (nombreCompleto ?? "").Trim();
        if (string.IsNullOrEmpty(n)) return ("", "");
        var parts = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return (parts[0], "");
        return (string.Join(" ", parts[..^1]), parts[^1]);
    }

    private static int ResolveRolId(string? rol, bool esAdminFlag)
    {
        if (esAdminFlag) return AppConstants.ROLID_SUPERADMIN;
        var r = (rol ?? "").Trim();

        if (r.Equals(AppConstants.ROL_ADMINISTRADOR, StringComparison.OrdinalIgnoreCase) || r.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            return AppConstants.ROLID_ADMINISTRADOR;
        if (r.Equals(AppConstants.ROL_CAJERO, StringComparison.OrdinalIgnoreCase))
            return AppConstants.ROLID_CAJERO;
        if (r.Equals(AppConstants.ROL_SUPERADMIN, StringComparison.OrdinalIgnoreCase))
            return AppConstants.ROLID_SUPERADMIN;

        return AppConstants.ROLID_CLIENTE;
    }
}
