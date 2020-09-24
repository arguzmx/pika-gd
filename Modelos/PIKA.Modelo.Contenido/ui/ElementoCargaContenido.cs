using Microsoft.AspNetCore.Http;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Contenido.ui
{

    public class ElementoCargaContenido
    {
        public ElementoCargaContenido()
        {
            Indice = 0;
        }

    
        /// <summary>
        /// contenido enviado en la forma
        /// </summary>
        [NotMapped]
        public IFormFile file { get; set; }

        /// <summary>
        /// Identificador único de la transacción para un grupo de envío
        /// </summary>
        public string TransaccionId { get; set; }


        /// <summary>
        /// Identificador único del volumen destino de la carga
        /// </summary>
        public string VolumenId { get; set; }

        /// <summary>
        /// Identificador único del elemento de contenido 
        /// </summary>
        public string ElementoId { get; set; }

        /// <summary>
        /// Identificador únido del punto de mentoja al que pertenece la solicitud
        /// </summary>
        public string PuntoMontajeId { get; set; }
        
        
        /// <summary>
        /// Version de contenido para el elemento en edición
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        ///  Indice para ordenar las adiciones en caso de diferencias 
        ///  en la velocidad de trasnferencia
        /// </summary>
        public int Indice { get; set; }


    }
}
