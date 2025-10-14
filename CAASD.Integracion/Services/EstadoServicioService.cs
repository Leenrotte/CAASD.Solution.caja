using System;
using System.Collections.Generic;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces;

namespace CAASD.Integracion.Services
{
    public class EstadoServicioService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IEnumerable<EstadoServicio> ObtenerEstadosActivos()
        {
            try
            {
                return _unitOfWork.EstadosServicio.GetActivos();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estados del servicio: {ex.Message}");
            }
        }

        public IEnumerable<EstadoServicio> ObtenerPorZona(string zona)
        {
            try
            {
                return _unitOfWork.EstadosServicio.GetByZona(zona);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estados por zona: {ex.Message}");
            }
        }

        public IEnumerable<EstadoServicio> ObtenerPorSector(string sector)
        {
            try
            {
                return _unitOfWork.EstadosServicio.GetBySector(sector);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estados por sector: {ex.Message}");
            }
        }

        public bool CrearInterrupcion(EstadoServicio estadoServicio)
        {
            try
            {
                estadoServicio.FechaInicio = DateTime.Now;
                estadoServicio.Activo = true;

                _unitOfWork.EstadosServicio.Add(estadoServicio);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear interrupción del servicio: {ex.Message}");
            }
        }

        public bool FinalizarInterrupcion(int id)
        {
            try
            {
                var estado = _unitOfWork.EstadosServicio.GetById(id);

                if (estado == null)
                    return false;

                estado.Activo = false;
                estado.FechaFinEstimada = DateTime.Now;

                _unitOfWork.EstadosServicio.Update(estado);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al finalizar interrupción: {ex.Message}");
            }
        }
    }
}