using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Define un espacio físico asociado al archivo donde se almacenan los activo físicos
    /// </summary>
    public class AlmacenArchivo : Entidad<string>, IEntidadNombrada
    {
        /// <summary>
        /// Nomnbre del almacém
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Clave asociada al arhchvi en la organización
        /// </summary>
        public string Clave { get; set; }

        /// <summary>
        /// Identificaodr único del archivo al qu pertenece el almacen
        /// </summary>
        public string ArchivoId { get; set; }


        public Archivo Archivo { get; set; }
    }
}
