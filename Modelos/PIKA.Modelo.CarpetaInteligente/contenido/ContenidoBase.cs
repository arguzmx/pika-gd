using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    /// <summary>
    /// Define las propieades básicas de los elementos contenidos en una carpeta
    /// </summary>
    public abstract class ContenidoBase : Entidad<Guid>, IEntidadNombrada
    {
        public ContenidoBase()
        {
            Permisos = new List<PermisoAcceso>();
        }

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

        /// <summary>
        /// Hereda los permisos de la entidad padre al momento de ser creada la entidad
        /// </summary>
        public bool HeredarPermisosPadre { get; set; }

        /// <summary>
        /// LIsta de permisos de acceso a la entidad
        /// </summary>
        public List<PermisoAcceso> Permisos { get; set; }
    }
}
