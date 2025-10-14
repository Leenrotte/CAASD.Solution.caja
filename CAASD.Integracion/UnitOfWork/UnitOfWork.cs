using System;
using Microsoft.EntityFrameworkCore.Storage;
using CAASD.Core.Interfaces;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;
using CAASD.Integracion.Repositories;

namespace CAASD.Integracion.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CAASDContext _context;
        private IDbContextTransaction? _transaction;
        private IUsuarioRepository? _usuarios;
        private IFacturaRepository? _facturas;
        private IReporteAveriaRepository? _reportesAveria;
        private IPagoRepository? _pagos;
        private INotificacionRepository? _notificaciones;
        private ISolicitudRepository? _solicitudes;
        private IContenidoEducativoRepository? _contenidosEducativos;
        private IEstadoServicioRepository? _estadosServicio;
        private IContratoRepository? _contratos;

        public UnitOfWork()
        {
            _context = new CAASDContext();
        }

        public UnitOfWork(CAASDContext context)
        {
            _context = context;
        }

        public IUsuarioRepository Usuarios
        {
            get
            {
                _usuarios ??= new UsuarioRepository(_context);
                return _usuarios;
            }
        }

        public IFacturaRepository Facturas
        {
            get
            {
                _facturas ??= new FacturaRepository(_context);
                return _facturas;
            }
        }

        public IReporteAveriaRepository ReportesAveria
        {
            get
            {
                _reportesAveria ??= new ReporteAveriaRepository(_context);
                return _reportesAveria;
            }
        }

        public IPagoRepository Pagos
        {
            get
            {
                _pagos ??= new PagoRepository(_context);
                return _pagos;
            }
        }

        public INotificacionRepository Notificaciones
        {
            get
            {
                _notificaciones ??= new NotificacionRepository(_context);
                return _notificaciones;
            }
        }

        public ISolicitudRepository Solicitudes
        {
            get
            {
                _solicitudes ??= new SolicitudRepository(_context);
                return _solicitudes;
            }
        }
        public IContratoRepository Contratos
        {
            get
            {
                _contratos ??= new ContratoRepository(_context);
                return _contratos;
            }
        }

        public IContenidoEducativoRepository ContenidosEducativos
        {
            get
            {
                _contenidosEducativos ??= new ContenidoEducativoRepository(_context);
                return _contenidosEducativos;
            }
        }

        public IEstadoServicioRepository EstadosServicio
        {
            get
            {
                _estadosServicio ??= new EstadoServicioRepository(_context);
                return _estadosServicio;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        
    }
}