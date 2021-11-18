using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{

    public abstract class ContenidoBase : Entidad<Guid>, IEntidadNombrada
    {

        /// <summary>
        /// Identificador único del contenido
        /// </summary>
        public override Guid Id { get; set; }
        
        /// <summary>
        /// Nombre del contenido al interior de la carpeta
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Tipo de contenido
        /// </summary>
        public TipoContenido Tipo { get; set; }

        /// <summary>
        /// Determina el órden de despliegue de los elementos
        /// </summary>
        public int IndiceDespliegue { get; set; }


    }
}
