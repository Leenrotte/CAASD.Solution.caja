using CAASD.Core.Constants;

namespace CAASD.Core.Helpers
{
    public static class AccessPolicy
    {
        /// <summary>
        /// Controla a qué módulo puede entrar cada rol según tu regla:
        /// - CORE : solo Superadmin
        /// - WEB  : Administrador y Cliente
        /// - CAJA : Cajero (proyecto aparte)
        /// </summary>
        public static bool RolPuedeEntrarModulo(int rolId, string moduloDestino)
        {
            if (moduloDestino == AppConstants.MOD_CORE)
                return rolId == AppConstants.ROLID_SUPERADMIN;

            if (moduloDestino == AppConstants.MOD_WEB)
                return rolId == AppConstants.ROLID_ADMINISTRADOR
                    || rolId == AppConstants.ROLID_CLIENTE;

            if (moduloDestino == AppConstants.MOD_CAJA)
                return rolId == AppConstants.ROLID_CAJERO;

            return false;
        }

        /// <summary>
        /// Solo el Superadmin crea usuarios de STAFF (Admin/Cajero) desde la consola del Core.
        /// </summary>
        public static bool PuedeCrearStaff(int rolId) =>
            rolId == AppConstants.ROLID_SUPERADMIN;

        /// <summary>
        /// Autoregistro de clientes habilitado en Web (no en Core).
        /// </summary>
        public static bool PuedeAutoregistroClienteEnWeb() => true;

        /// <summary>
        /// En Web, quién puede dar mantenimiento a perfiles de clientes.
        /// (Tu regla: Administrador; si quieres incluir Superadmin en Web, cambia aquí.)
        /// </summary>
        public static bool PuedeGestionarPerfilesCliente(int rolId) =>
            rolId == AppConstants.ROLID_ADMINISTRADOR;
    }
}
