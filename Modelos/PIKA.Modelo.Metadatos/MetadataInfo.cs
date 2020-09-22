using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    public class MetadataInfo
    {

        public MetadataInfo()
        {
            Reportes = new HashSet<IProveedorReporte>();
            Propiedades = new List<Propiedad>();
            EntidadesVinculadas = new List<EntidadVinculada>();
            CatalogosVinculados = new List<CatalogoVinculado>();
            VistasVinculadas = new List<LinkVista>();
        }

        /// <summary>
        /// Tipo de elemento basado en el nombre del ensamblado
        /// </summary>
       public string Tipo { get; set; }

        /// <summary>
        /// Fullname del tipo desde el ensamblado
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Especifica si el elimnado de la entidad ocurre a nivel lógico
        /// </summary>
        public bool ElminarLogico { get; set; }

        /// <summary>
        /// Especifica si hay una columna que controle el estaoo activo/inactivo de la enteidad
        /// </summary>
        public bool OpcionActivarDesativar { get; set; }


        /// <summary>
        /// Si OpcionActivr es true este campo defina la columna logica para establecer el estado
        /// </summary>
        public string ColumnaActivarDesativar { get; set; }

        /// <summary>
        /// Especifica si la entidad requiere paginado relacional
        /// </summary>
        public bool PaginadoRelacional { get; set; }

        /// <summary>
        /// Identifica la columna utilizada para determinar la eliminación lógica
        /// </summary>
        public string ColumaEliminarLogico { get; set; }
        
        public virtual List<Propiedad> Propiedades { get; set; }
        
        public virtual List<EntidadVinculada> EntidadesVinculadas { get; set; }
        
        public virtual List<CatalogoVinculado> CatalogosVinculados { get; set; }

        public virtual List<LinkVista> VistasVinculadas { get; set; }

        public ICollection<IProveedorReporte> Reportes { get; set; }
    }
}
