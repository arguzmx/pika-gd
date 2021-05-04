using Nest;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.ServicioBusqueda.Contenido
{



    public class BusquedaContenido: ParametrosConsulta
    {
        /// <summary>
        /// Identificador único de la búsqueda
        /// </summary>
        [Keyword]
        public string  Id { get; set; }
        
        [Object]
        public List<Busqueda> Elementos { get; set; }
        
        [Date]
        public DateTime Fecha { get; set; }

        [Date]
        public DateTime FechaFinalizado { get; set; }

        [Number(NumberType.Integer)]
        public EstadoBusqueda Estado { get; set; }

        [Keyword]
        public string PuntoMontajeId { get; set; }

        [Keyword]
        public string PlantillaId { get; set; }

    }


   public class Busqueda
    {
        /// <summary>
        /// Tag de la busqda a realizar, ej, elemento, folder, metadatos
        /// </summary>
        [Keyword]
        public string Tag { get; set; }

        /// <summary>
        /// Tema de la bpsqueda por ejemplo Id de la plantilla o nulo para los elementos, carpetas, etc
        /// ste valro se encuentra vinculado al Tag que define el tipo de busqueda
        /// </summary>
        [Keyword]
        public string Topico { get; set; }
        
        [Object]
        public Consulta Consulta { get; set; }
        
        /// <summary>
        /// total de elementos encontrado en la bsqueda al momento de la ejecucuión
        /// </summary>
        [Number(NumberType.Long)]
        public long Conteo { get; set; }

        /// <summary>
        /// Lista de identificadores utilizados para la interseccón en el caso de b´psquedas compuestas
        /// debe esta limitado para evitar problmas de desempeño
        /// </summary>
        [Object]
        public List<string> Ids { get; set; }

    }

}
