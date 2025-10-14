using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace CAASD.Core.Constants
{
    public static class AppConstants
    {
        // ===== Roles (nombres) =====
        public const string ROL_SUPERADMIN = "Superadmin";
        public const string ROL_ADMINISTRADOR = "Administrador";
        public const string ROL_CLIENTE = "Cliente";
        public const string ROL_CAJERO = "Cajero";

        

        // ===== Roles (IDs semilla) =====
        // Estos IDs deben coincidir con los que siembres en tu CAASDContext.
        public const int ROLID_SUPERADMIN = 1;
        public const int ROLID_ADMINISTRADOR = 2;
        public const int ROLID_CLIENTE = 3;
        public const int ROLID_CAJERO = 4;


        // ===== Módulos (para enrutamiento de login) =====
        public const string MOD_CORE = "Core";
        public const string MOD_WEB = "Web";
        public const string MOD_CAJA = "Caja";

        // ===== Estados =====
        public const string ESTADO_ACTIVO = "Activo";
        public const string ESTADO_INACTIVO = "Inactivo";
        public const string ESTADO_PENDIENTE = "Pendiente";
        public const string ESTADO_PROCESADO = "Procesado";

        // ===== Métodos de Pago =====
        public const string METODO_PAGO_TARJETA = "Tarjeta";
        public const string METODO_PAGO_TRANSFERENCIA = "Transferencia";
        public const string METODO_PAGO_EFECTIVO = "Efectivo";

        // ===== Estados de Pago =====
        public const string PAGO_PENDIENTE = "Pendiente";
        public const string PAGO_COMPLETADO = "Completado";
        public const string PAGO_FALLIDO = "Fallido";
        public const string PAGO_CANCELADO = "Cancelado";

        // ===== Estados de Avería =====
        public const string AVERIA_REPORTADA = "Reportada";
        public const string AVERIA_EN_PROCESO = "EnProceso";
        public const string AVERIA_RESUELTA = "Resuelta";
        public const string AVERIA_CERRADA = "Cerrada";

        // ===== Prioridades =====
        public const string PRIORIDAD_BAJA = "Baja";
        public const string PRIORIDAD_MEDIA = "Media";
        public const string PRIORIDAD_ALTA = "Alta";
        public const string PRIORIDAD_CRITICA = "Critica";

        // ===== Tipos de Avería =====
        public const string AVERIA_FUGA = "Fuga";
        public const string AVERIA_SIN_AGUA = "SinAgua";
        public const string AVERIA_BAJA_PRESION = "BajaPresion";
        public const string AVERIA_ALCANTARILLADO = "Alcantarillado";

        // ===== Tipos de Solicitud =====
        public const string SOLICITUD_NUEVO_CONTRATO = "NuevoContrato";
        public const string SOLICITUD_CAMBIO_TITULAR = "CambioTitular";
        public const string SOLICITUD_CANCELACION = "Cancelacion";
        public const string SOLICITUD_RECLAMO = "Reclamo";

        // ===== Configuraciones =====
        public const int RADIO_BUSQUEDA_KM = 5;
        public const int DIAS_VENCIMIENTO_FACTURA = 15;
        public const decimal ITBIS_PORCENTAJE = 0.18m;
    }
}
