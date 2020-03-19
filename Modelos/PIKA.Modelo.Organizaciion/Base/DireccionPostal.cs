using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// Detalla la ubicació espacial de un elemento en forma de una dirección postal
    /// Las direcciones postales se  enlazan a los objeto a através de una entidad de relación
    /// con la finalidad de establecer relaciones varios a varios
    /// </summary>
    public class DireccionPostal : Entidad<string>, IEntidadNombrada, IEntidadRelacionada
    {
     
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_NULO;


        public DireccionPostal()
        {
            this.TipoOrigenId = this.TipoOrigenDefault;
        }

        /// <summary>
        /// NOmbre corto para reconocer la dirección por ejemplo: Casa, Oficina, etc
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Calle de la dirección
        /// </summary>
        public string Calle { get; set; }

        /// <summary>
        /// No. interior de la dirección
        /// </summary>
        public string NoInterno { get; set; }


        /// <summary>
        /// No. exterior de la dirección
        /// </summary>
        public string NoExterno { get; set; }


        /// <summary>
        /// Colonia o barrio de la dirección
        /// </summary>
        public string Colonia { get; set; }


        /// <summary>
        /// Código postal de la dirección
        /// </summary>
        public string CP { get; set; }


        /// <summary>
        /// Municipio o alcaldía de la dirección
        /// </summary>
        public string Municipio { get; set; }


        /// <summary>
        /// Estado del país de la dirección
        /// </summary>
        public string EstadoId { get; set; }


        /// <summary>
        /// País de la dirección
        /// </summary>
        public string PaisId { get; set; }



        /// <summary>
        /// Tipo de ojeto origen asociado a la dirección
        /// </summary>
        public string TipoOrigenId { get; set; }


        /// <summary>
        /// Identificador único del objeto origen
        /// </summary>
        public string OrigenId { get; set; }


        public virtual Pais Pais { get; set; }

        public virtual Estado Estado { get; set; }
    }
}
