using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class Plantilla : Entidad<string>, IEntidadNombrada, IEntidadRelacionada, IEntidadEliminada
    {
    
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        public Plantilla()
        {
            this.Propiedades = new HashSet<Propiedad>();
            this.TipoOrigenId = TipoOrigenDefault;
        }

       

        /// <summary>
        /// Nombre de la plantilla
        /// </summary>
        public string Nombre { get ; set; }

        /// <summary>
        /// Identificador de relación de origem, en este caso se utiliza
        /// para vincular la plantilla con su dominio 
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identficador único del dominio al que pertenece la plantilla
        /// El Id de ralción es el identificador de un dominio
        /// </summary>
        public string OrigenId { get; set; }

        /// <summary>
        /// Establece si la plantilla ha sido eliminada
        /// </summary>
        public bool Eliminada { get; set; }


        public ICollection<Propiedad> Propiedades { get; set; }

       
    }
}
