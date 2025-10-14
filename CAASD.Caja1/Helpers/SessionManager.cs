using CAASD.Core.DTOs;

namespace CAASD.Caja1.Helpers
{
    public static class SessionManager
    {
        public static UsuarioDTO UsuarioActual { get; set; }
        public static bool EstaLogueado => UsuarioActual != null;
        public static bool EsAdministrador => UsuarioActual?.EsAdministrador ?? false;

        public static void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
}