using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{


    public class Archivo : Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRelacionada
    {


        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;

        /// <summary>
        /// Espacio físico en el que se deposita la 
        /// documentación que lo conforma, es decir, el archivo como
        /// depósito.
        /// </summary>
        public Archivo()
        {
            this._TipoOrigenId = this.TipoOrigenDefault;
            this.Almacenes = new HashSet<AlmacenArchivo>();
            HistorialArchivosActivo = new HashSet<HistorialArchivoActivo>();
            Prestamos = new HashSet<Prestamo>();

        }


        /// <summary>
        /// Nombre único del cuadro de clasificación
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Especifica si el elemento ha sido marcado como eliminado
        /// </summary>
        public bool Eliminada { get; set; }



        private string _TipoOrigenId;
        /// <summary>
        /// El tipo de orígen en para este modelo es el elemento de la unidad organizacional 
        /// Este elemento puede ser unn departamento u oficina que tiene acervo as su cargo
        /// </summary>
        public string TipoOrigenId
        {
            get { return _TipoOrigenId; }
            set
            {
                // el valor no debe cambiarse

            }
        }

        /// <summary>
        /// Identificador de la organización a la que pertenece el cuadro de clasificación
        /// </summary>
        public string OrigenId { get; set; }


        /// <summary>
        /// IDentificador del tipo de archivo
        /// </summary>
        public string TipoArchivoId { get; set; }


        /// <summary>
        /// Tipo de archivo 
        /// </summary>
        public virtual TipoArchivo Tipo { get; set; }
        public virtual ICollection<AlmacenArchivo> Almacenes { get; set; }
        public virtual Activo ActivoActual { get; set; }
        public virtual ICollection<HistorialArchivoActivo> HistorialArchivosActivo { get; set; }
        public virtual ICollection<Prestamo> Prestamos { get; set; }

    }
}
