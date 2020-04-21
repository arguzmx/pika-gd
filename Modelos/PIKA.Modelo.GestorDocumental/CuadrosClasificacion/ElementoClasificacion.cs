using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class ElementoClasificacion : Entidad<string>, IEntidadNombrada, IEntidadEliminada
    {

        public ElementoClasificacion() {
            Padre = null;
            Hijos = new HashSet<ElementoClasificacion>();
            Activos = new HashSet<Activo>();
        }

        /// <summary>
        /// Padre  del elemento actual 
        /// </summary>
        public string ElementoClasificacionId { get; set; }

        /// <summary>
        /// Clave asociada el elemento de clasifaición por ejemplo 1.S
        /// </summary>
        public string Clave { get; set; }

        /// <summary>
        /// Nombre del elemento de clasifiación debe ser único en para los elemntos de un mismo padre
        /// </summary>
        public string Nombre { get; set; }
        
        /// <summary>
        /// Indica si el elemento ha sido marcado para eliminación
        /// </summary>
        public bool Eliminada { get; set; }


        /// <summary>
        /// Determina la posición relativa del elemento en relación con los elementos del mismo nivle
        /// </summary>
        public int Posicion { get; set; }

        /// <summary>
        /// Cuadro de clasificación al que pertenecen los elementos
        /// </summary>
        public string CuadroClasifiacionId { get; set; }

        /// <summary>
        /// Elemento padre del actual
        /// </summary>
        public virtual ElementoClasificacion Padre { get; set; }

        /// <summary>
        /// Elementos descencientes del elemento actual
        /// </summary>
        public virtual ICollection<ElementoClasificacion> Hijos { get; set; }

        /// <summary>
        /// Instancia del cuadro de clasificación
        /// </summary>
        public virtual CuadroClasificacion CuadroClasificacion { get; set; }
        /// <summary>
        /// Activos del elemento clasificacion
        /// </summary>
        public virtual ICollection<Activo> Activos { get; set; }
    }
}
