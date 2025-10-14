using System;
using System.Collections.Generic;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces;

namespace CAASD.Integracion.Services
{
    public class ContenidoEducativoService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IEnumerable<ContenidoEducativo> ObtenerContenidosActivos()
        {
            try
            {
                return _unitOfWork.ContenidosEducativos.GetActivos();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener contenidos educativos: {ex.Message}");
            }
        }

        public IEnumerable<ContenidoEducativo> ObtenerPorCategoria(string categoria)
        {
            try
            {
                return _unitOfWork.ContenidosEducativos.GetByCategoria(categoria);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener contenidos por categoría: {ex.Message}");
            }
        }

        public ContenidoEducativo? ObtenerPorId(int id)
        {
            try
            {
                return _unitOfWork.ContenidosEducativos.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener contenido educativo: {ex.Message}");
            }
        }

        public bool CrearContenido(ContenidoEducativo contenido)
        {
            try
            {
                contenido.FechaPublicacion = DateTime.Now;
                contenido.Activo = true;

                _unitOfWork.ContenidosEducativos.Add(contenido);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear contenido educativo: {ex.Message}");
            }
        }
    }
}