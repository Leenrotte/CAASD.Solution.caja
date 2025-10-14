using System;
using System.Linq;
using CAASD.Core.DTOs;
using CAASD.Core.Entities;
using CAASD.Core.Exceptions;
using CAASD.Core.Helpers;
using CAASD.Core.Interfaces;
using CAASD.Core.Interfaces.Services;


namespace CAASD.Integracion.Services
{
    public class AuthService(IUnitOfWork unitOfWork) : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public LoginResponseDTO Login(LoginRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Mensaje = "Usuario y contraseña son requeridos"
                    };
                }

                var usuario = _unitOfWork.Usuarios.GetByEmail(request.Email);

                if (usuario == null)
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Mensaje = "Credenciales inválidas"
                    };
                }

                if (!usuario.Activo)
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Mensaje = "Usuario inactivo. Contacte al administrador"
                    };
                }

                string passwordHash = SecurityHelper.HashPassword(request.Password);

                if (usuario.PasswordHash != passwordHash)
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Mensaje = "Credenciales inválidas"
                    };
                }

                // Verifica que el rol del usuario pueda acceder al módulo solicitado
                if (!AccessPolicy.RolPuedeEntrarModulo(usuario.RolId, request.ModuloDestino))
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Mensaje = $"No autorizado para el módulo {request.ModuloDestino}."
                    };
                }

                usuario.UltimoAcceso = DateTime.Now;
                _unitOfWork.Usuarios.Update(usuario);
                _unitOfWork.SaveChanges();

                string token = SecurityHelper.GenerarToken();


                return new LoginResponseDTO
                {
                    Success = true,
                    Token = token,
                    Mensaje = "Login exitoso",
                    Usuario = new UsuarioDTO
                    {
                        Id = usuario.Id,
                        Cedula = usuario.Cedula,
                        NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                        Email = usuario.Email,
                        Telefono = usuario.Telefono,
                        Rol = usuario.Rol?.Nombre ?? "Cliente",
                        EsAdministrador = usuario.Rol?.EsAdministrador ?? false
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Mensaje = $"Error al iniciar sesión: {ex.Message}"
                };
            }
        }

        public bool RegistrarUsuario(UsuarioDTO usuarioDto, string password)
        {
            try
            {
                if (_unitOfWork.Usuarios.GetByEmail(usuarioDto.Email) != null)
                {
                    throw new AutenticacionException("El email ya está registrado");
                }

                if (_unitOfWork.Usuarios.GetByCedula(usuarioDto.Cedula) != null)
                {
                    throw new AutenticacionException("La cédula ya está registrada");
                }

                var usuario = new Usuario
                {
                    Cedula = usuarioDto.Cedula,
                    Nombre = usuarioDto.NombreCompleto.Split(' ')[0],
                    Apellido = usuarioDto.NombreCompleto.Contains(' ')
                        ? usuarioDto.NombreCompleto[(usuarioDto.NombreCompleto.IndexOf(' ') + 1)..]
                        : string.Empty,
                    Email = usuarioDto.Email,
                    Telefono = usuarioDto.Telefono,
                    DireccionCompleta = usuarioDto.Direccion,
                    PasswordHash = SecurityHelper.HashPassword(password),
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    RolId = 3 // Cliente por defecto
                };

                _unitOfWork.Usuarios.Add(usuario);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : "Sin inner exception";
                throw new AutenticacionException($"Problema en AuthService: {ex.Message} | Detalle: {inner}");
            }
        }
        

        public bool CambiarPassword(int usuarioId, string passwordActual, string passwordNuevo)
        {
            try
            {
                var usuario = _unitOfWork.Usuarios.GetById(usuarioId);

                if (usuario == null)
                {
                    throw new AutenticacionException("Usuario no encontrado");
                }

                string passwordActualHash = SecurityHelper.HashPassword(passwordActual);

                if (usuario.PasswordHash != passwordActualHash)
                {
                    throw new AutenticacionException("La contraseña actual es incorrecta");
                }

                usuario.PasswordHash = SecurityHelper.HashPassword(passwordNuevo);
                _unitOfWork.Usuarios.Update(usuario);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new AutenticacionException($"Error al cambiar contraseña: {ex.Message}");
            }
        }

        public bool RecuperarPassword(string email)
        {
            try
            {
                var usuario = _unitOfWork.Usuarios.GetByEmail(email);

                if (usuario == null)
                {
                    return true;
                }

                string codigoVerificacion = SecurityHelper.GenerarCodigoVerificacion();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool RegistrarUsuarioAdmin(UsuarioDTO usuarioDto, string password, int rolId)
        {
            try
            {
                if (rolId != 2 && rolId != 4)
                    throw new AutenticacionException("Solo se pueden crear roles de Administrador (2) o Cajero (4).");

                if (_unitOfWork.Usuarios.GetByEmail(usuarioDto.Email) != null)
                    throw new AutenticacionException("El correo ya está registrado.");

                if (_unitOfWork.Usuarios.GetByCedula(usuarioDto.Cedula) != null)
                    throw new AutenticacionException("La cédula ya está registrada.");

                var usuario = new Usuario
                {
                    Cedula = usuarioDto.Cedula,
                    Nombre = usuarioDto.NombreCompleto.Split(' ')[0],
                    Apellido = usuarioDto.NombreCompleto.Contains(' ')
                        ? usuarioDto.NombreCompleto[(usuarioDto.NombreCompleto.IndexOf(' ') + 1)..]
                        : string.Empty,
                    Email = usuarioDto.Email,
                    Telefono = usuarioDto.Telefono,
                    DireccionCompleta = usuarioDto.Direccion,
                    PasswordHash = SecurityHelper.HashPassword(password),
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    RolId = rolId
                };

                _unitOfWork.Usuarios.Add(usuario);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new AutenticacionException($"Error al crear usuario administrativo: {ex.Message}");
            }
        }
    }

}
