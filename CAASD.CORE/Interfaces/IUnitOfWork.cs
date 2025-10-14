using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Interfaces.Repositories;


namespace CAASD.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository Usuarios { get; }
        IContratoRepository Contratos { get; }
        IFacturaRepository Facturas { get; }
        IReporteAveriaRepository ReportesAveria { get; }
        IPagoRepository Pagos { get; }
        INotificacionRepository Notificaciones { get; }
        ISolicitudRepository Solicitudes { get; }
        IContenidoEducativoRepository ContenidosEducativos { get; }
        IEstadoServicioRepository EstadosServicio { get; }

        int SaveChanges();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}