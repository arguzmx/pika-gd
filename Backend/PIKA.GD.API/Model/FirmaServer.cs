using System;

namespace PIKA.GD.API.Model
{
    /// <summary>
    /// Clase para licenciamiento de la instancia de PIKA
    /// </summary>
    public class FirmaServer
    {
        /// <summary>
        /// NOmbre del host
        /// </summary>
        public string HostName { get; set; }
        
        /// <summary>
        /// Identificador único del server
        /// </summary>
        public Guid IdServer { get; set; }
        
        /// <summary>
        /// Clave de activación
        /// </summary>
        public Guid? Activacion { get; set; }
        
        /// <summary>
        /// Fcha de la activación
        /// </summary>
        public DateTime? FechaActivacion { get; set; }

    }
}
