using Microsoft.AspNetCore.Routing.Internal;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static PIKA.Modelo.Metadatos.PropAttribute;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Define una propiedad de meatdatos, se basa en el atributo web PIKA.modelo.Metadatos.PropAttribue
    /// Culauqier cambio en alguno de los 2 debe sincornizarse manualemnte
    /// </summary>
    public class Propiedad : Entidad<string>, IEntidadNombrada
    {

        public Propiedad()
        {
            this.OrdenarValoresListaPorNombre = true;
            this.ValoresLista = new HashSet<ValorLista>();
            this.AtributosEvento = new List<AtributoEvento>();
            this.AtributosVistaUI = new List<AtributoVistaUI>();
            this.ValorDefault = null;
        }

        /// <summary>
        /// Identificador único de la propiedad
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Nombre asociado a la propiedad
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Identificador del tipo de propiedad
        /// </summary>
        public string TipoDatoId { get; set; }


        /// <summary>
        /// Valor por defecto de la propiedad serializado como string
        /// en el caso de los binarios debe serializarrse como base 64
        /// </summary>
        public string ValorDefault { get; set; }

        /// <summary>
        /// Índice de despliegue para la propiedad
        /// </summary>
        public int IndiceOrdenamiento { get; set; }


        /// <summary>
        /// establece la posición del campo en el despliegue tabular
        /// </summary>
        [NotMapped]
        public int IndiceOrdenamientoTabla { get; set; }

        /// <summary>
        /// Determina si es posible realizar bpsuquedas sobre la propiedad
        /// </summary>
        public bool Buscable { get; set; }

        /// <summary>
        /// Determina si es posible realizar ordenamiento sobre la propiedad
        /// </summary>
        public bool Ordenable { get; set; }

        /// <summary>
        /// Determina si la propieda es vivisble
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// DEtermina si es una clave primaria
        /// </summary>
        public bool EsIdClaveExterna { get; set; }



        /// <summary>
        /// Dettermina si es el identificador de un registro
        /// </summary>
        public bool EsIdRegistro { get; set; }



        /// <summary>
        /// Dettermina si es el identificador de una jeraquía
        /// </summary>
        public bool EsIdJerarquia { get; set; }

        /// <summary>
        /// Dettermina si coreresponde al texto de un registro
        /// </summary>
        public bool EsTextoJerarquia { get; set; }

        /// <summary>
        /// Dettermina si coreresponde a la propiedad padre para una relacion jerárquica
        /// </summary>
        public bool EsIdRaizJerarquia { get; set; }


        /// <summary>
        /// Dettermina si coreresponde a la propiedad padre para una relacion jerárquica
        /// </summary>
        public bool EsFiltroJerarquia { get; set; }


        /// <summary>
        /// DEtermina si es requerido
        /// </summary>
        public bool Requerido { get; set; }

        /// <summary>
        /// Determina si el valos es autogenerado
        /// </summary>
        public bool Autogenerado { get; set; }

        /// <summary>
        /// Determina si el valos es un campo de índice 
        /// </summary>
        public bool EsIndice { get; set; }


        public bool OrdenarValoresListaPorNombre { get; set; }

        /// <summary>
        /// Especifica el control de HTML preferido
        /// </summary>
        public string ControlHTML { get; set; }


        /// <summary>
        /// Determina si el contenido de l apropiedad puede utilizaerse como etiqueta de despliegie
        /// para humanos
        /// </summary>
        public bool Etiqueta { get; set; }

        /// <summary>
        /// Especifica si al propiedad debe inferirse del contexto de ejecución
        /// </summary>
        [NotMapped]
        public bool Contextual { get; set; }

        [NotMapped]
        public string IdContextual { get; set; }


        [NotMapped]
        public bool MostrarEnTabla { get; set; }

        [NotMapped]
        public bool AlternarEnTabla { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual TipoDato TipoDato { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual AtributoLista AtributoLista { get; set; }

        public virtual AtributoTabla AtributoTabla { get; set; }
        [XmlIgnore]
        [JsonIgnore]

        public virtual List<AtributoEvento> AtributosEvento { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual List<AtributoVistaUI> AtributosVistaUI { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual ValidadorTexto ValidadorTexto { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual ValidadorNumero ValidadorNumero { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public ICollection<ValorLista> ValoresLista { get; set; }

        

    }
}
