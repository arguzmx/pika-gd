using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Corresponde a una instacia que permite la clasificación documental
    /// </summary>
    public class EntradaClasificacion : Entidad<string>, IEntidadNombrada, IEntidadEliminada
    {

        public EntradaClasificacion()
        {
            ValoracionesEntrada = new HashSet<ValoracionEntradaClasificacion>();
        }

        /// <summary>
        ///  Identificador único del la entrada del cuadro de clasificación
        /// </summary>
        public override string Id { get; set; }


        /// <summary>
        /// Identificador único del elemento de clasificación al que pertenece la entrada
        /// </summary>
        public string ElementoClasificacionId { get; set; }

        /// <summary>
        /// Clave para la entrada del cuandro, generalmente comienza con la del elemento de clasificación
        /// </summary>
        public string Clave { get; set; }

        /// <summary>
        /// Nombre de la entrada
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Determina si la entrada del cuadro ha sido eliminada
        /// </summary>
        public bool Eliminada { get; set; }

        /// <summary>
        /// Determina la posición relativa del elemento en relación con los elementos del mismo nivle
        /// </summary>
        public int Posicion { get; set; }

        /// <summary>
        /// Especifica los meses que debe permanecer el expediente o documento en el archivo de trámite una vez que ha sido cerrado
        /// </summary>
        public int MesesVigenciTramite { get; set; }

        /// <summary>
        /// Especifica los meses que debe permanecer el expediente o documento en el archivo de concentración una vez que ha sido cerrado
        /// </summary>
        public int MesesVigenciConcentracion { get; set; }

        /// <summary>
        /// Especifica los meses que debe permanecer el expediente o documento en el archivo de histórico una vez que ha sido cerrado
        /// </summary>
        public int MesesVigenciHistorico { get; set; }

        /// <summary>
        /// Identificador único del tipo de disposición documental para la entrada
        /// </summary>
        public string TipoDisposicionDocumentalId { get; set; }

        public ICollection<ValoracionEntradaClasificacion> ValoracionesEntrada { get; set; }

        public TipoDisposicionDocumental DisposicionEntrada { get; set; }

    }
}
