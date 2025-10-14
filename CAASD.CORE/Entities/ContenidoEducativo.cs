using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class ContenidoEducativo
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public bool Activo { get; set; }
    }
}
