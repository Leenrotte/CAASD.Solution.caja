using CAASD.Core.Constants;

namespace CAASD.Core.Helpers
{
    public static class AccessPolicy
    {
        public static bool RolPuedeEntrarModulo(int rolId, string moduloDestino)
        {
            return (rolId == AppConstants.ROLID_SUPERADMIN && moduloDestino == AppConstants.MOD_CORE)
                || (rolId == AppConstants.ROLID_ADMINISTRADOR && moduloDestino == AppConstants.MOD_WEB)
                || (rolId == AppConstants.ROLID_CLIENTE && moduloDestino == AppConstants.MOD_WEB)
                || (rolId == AppConstants.ROLID_CAJERO && moduloDestino == AppConstants.MOD_CAJA);
        }
    }
}
