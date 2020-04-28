using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Define el espacio físico al que serán anexados las partes de un contenido
    /// </summary>
    public class Volumen: Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRelacionada
    {

        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;
        
        public Volumen() {
            this.TipoOrigenId = TipoOrigenDefault;
        }

        /// <summary>
        /// Tipo de origen para el volumen puede ser la Unidaorganizacionla o el Dominio dependiendo del grado de 
        /// visibilidad del volumen
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del origen, por ejempl el ID de la UO o el dominio
        /// </summary>
        public string OrigenId { get; set; }

        /// <summary>
        /// Indica si el volumen se encuentra activo 
        /// </summary>
        public bool Activo { get; set; }
        //Requediro defautl =true

        /// <summary>
        /// Especifica si el volumen se encuntra habilitado para escritura
        /// </summary>
        public bool EscrituraHabilitada { get; set; }
        //Requediro defautl =false

            /// <summary>
            /// Idemntificadorúnido del tipo de gestor
            /// </summary>
        public string TipoGestorESId { get; set; }

        /// <summary>
        /// Datos serializados para el acceso de lectura y excritea del Volumen
        /// </summary>
        public string  CadenaConexion { get; set; }
        //# longitud 2000, es requerida

        /// <summary>
        /// Nombre únicop del volumen
        /// </summary>
        public string Nombre { get; set; }
        //# longitud nombre, es requerida

        /// <summary>
        /// Identifica sie l volumen ha sido marcado como eliminadp
        /// </summary>
        public bool Eliminada { get; set; }
        //Requediro defautl =false


            /// <summary>
            /// Consecutivo del elemento para el alamcenamiento, esta propieda tambié existe en la Parte del contenido 
            /// para poder asociar un Id de tipo String, con uno númerio si es necesario
            /// </summary>
        public long ConsecutivoVolumen { get; set; }

        /// <summary>
        /// Número de partess contenidas en el volumen
        /// </summary>
        public long CanidadPartes { get; set; }

        /// <summary>
        /// Tamaño de volumen en bytes
        /// </summary>
        public long Tamano { get; set; }

        /// <summary>
        /// Esta propedad eno debe serializarse en la base de datos
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public TipoGestorES GestorES { get; set; }

        

        
    }

}
