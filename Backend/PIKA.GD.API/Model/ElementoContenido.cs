using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Model
{
    public class ElementoContenido
    {
        public ElementoContenido()
        {
            Indice = 0;
        }

        public IFormFile file { get; set; }
        public string VolumenId { get; set; }
        public string ElementoId { get; set; }
        public string PuntoMontajeId { get; set; }
        
        /// <summary>
        ///  Indice para ordenar las adiciones en caso de diferencias 
        ///  en la velocidad de trasnferencia
        /// </summary>
        public int Indice { get; set; }
    }
}
