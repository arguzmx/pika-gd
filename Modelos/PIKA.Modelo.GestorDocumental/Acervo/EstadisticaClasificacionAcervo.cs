using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class EstadisticaClasificacionAcervo
    {

        /// <summary>
        /// Identifiacor únio del archivo al que pertenece el acervo
        /// </summary>
        public string ArchivoId{ get; set; }


        /// <summary>
        /// IDentifiacor únio del cuadro de clasificacion
        /// </summary>
        public string CuadroClasificacionId { get; set; }


        /// <summary>
        /// Identificador de la entrada de clasificacion
        /// </summary>
        public string EntradaClasificacionId { get; set; }


        
        /// <summary>
        /// Cantidad de activos existentes en el archivo queperteneen a la entrada clasificación, 
        /// sólo contailizan los qu eno han sido eliminados
        /// </summary>
       public int ConteoActivos { get; set; }

        /// <summary>
        /// Cantidad de activos marcados como eliminados que pertenecen al archivo
        /// </summary>
        public int ConteoActivosEliminados { get; set; }
        /// <summary>
        /// Fecha de minima de apertura del activo del archivo
        /// </summary>
        public DateTime? FechaMinApertura { get; set; }
        /// <summary>
        /// Fecha de máxima de cierre del activo del archivo
        /// </summary>
        public DateTime? FechaMaxCierre { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Archivo Archivo { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public CuadroClasificacion CuadroClasificacion { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public EntradaClasificacion EntradaClasificacion { get; set; }

    }
}
