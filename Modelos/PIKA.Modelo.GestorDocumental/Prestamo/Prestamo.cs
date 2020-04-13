using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class Prestamo: Entidad<string>, IEntidadEliminada
    {

        public Prestamo() {
            Activos = new HashSet<ActivoPrestamo>();
            Comentarios = new HashSet<ComentarioPrestamo>();
        }

        public override string Id { get; set; }
        public bool Eliminada { get; set; }

        /// <summary>
        /// Identificador único del archivo que hizo la solicitud
        /// </summary>
        public string ArchivoId { get; set; }

        /// <summary>
        /// FEcha de creación del regiustro debe ser la fecha del sistema UTC
        /// </summary>
        public DateTime FechaCreacion { get; set; }


        /// <summary>
        /// Fecha programda parala devolucion del preátsmo en formato UTC
        /// </summary>
        public DateTime FechaProgramadaDevolucion { get; set; }


        /// <summary>
        /// FEcha de devolcuión final del préstamo
        /// </summary>
        public DateTime FechaDevolucion { get; set; }


        /// <summary>
        /// Indica si ha habido devoluciones parciales del préstamo
        /// </summary>
        public bool TieneDevolucionesParciales { get; set; }

        /// <summary>
        /// Número de fplio del préstamo
        /// </summary>
        public string Folio { get; set; }
        //# 100 caracteres


        public Archivo Archivo { get; set; }

        public virtual ICollection<ActivoPrestamo> Activos { get; set; }

        public virtual ICollection<ComentarioPrestamo> Comentarios { get; set; }

    }
}
