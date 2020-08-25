using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{
    public class MetadataInfo
    {
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
        [XmlIgnore]
        [JsonIgnore]
        public virtual List<Propiedad> Propiedades { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual List<EntidadVinculada> EntidadesVinculadas { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual List<CatalogoVinculado> CatalogosVinculados { get; set; }


    }
}
