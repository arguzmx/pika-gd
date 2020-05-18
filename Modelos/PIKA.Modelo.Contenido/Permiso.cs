using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{

    /// <summary>
    /// Permiso aplicable al contenido 
    /// </summary>
    public class Permiso: Entidad<string>
    {

        public Permiso()
        {

            Destinatarios = new HashSet<DestinatarioPermiso>();
            Carpetas = new HashSet<Carpeta>();
            Elementos = new HashSet<Elemento>();
        }

        /// <summary>
        /// Identificador único del contenido
        /// </summary>
        public override string Id { get; set; }

        public bool Leer { get; set; }

        public bool Escribir { get; set; }

        public bool Crear { get; set; }

        public bool Eliminar { get; set; }

        public virtual ICollection<DestinatarioPermiso> Destinatarios { get; set; }

        public virtual ICollection<Carpeta> Carpetas { get; set; }

        public virtual ICollection<Elemento> Elementos { get; set; }
    }
}
