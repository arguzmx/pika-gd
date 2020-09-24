using PIKA.Modelo.Contenido.ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class ElementoTransaccionCarga: ElementoCargaContenido
    {
    
        /// <summary>
        /// Identificador único del contenido
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre Orgin
        /// </summary>
        public string NombreOriginal { get; set; }

        /// <summary>
        /// Nombre Orgin
        /// </summary>
        public long TamanoBytes { get; set; }

        /// <summary>
        /// fecha de registro de la carga
        /// </summary>
        public DateTime FechaCarga { get; set; }

        /// <summary>
        /// fecha de proceso del contenido
        /// </summary>
        public DateTime? FechaProceso { get; set; }

        /// <summary>
        /// Estado de proceso finalizado
        /// </summary>
        public bool Procesado { get; set; }

        /// <summary>
        /// Error al procesar
        /// </summary>
        public bool Error { get; set; }
        
        /// <summary>
        /// Información del error a proesar
        /// </summary>
        public string Info { get; set; }

    }
}
