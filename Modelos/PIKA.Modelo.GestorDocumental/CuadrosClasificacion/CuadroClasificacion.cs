using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class CuadroClasificacion : Entidad<string>, IEntidadNombrada, IEntidadEliminada, IEntidadRelacionada
    {

       

        public CuadroClasificacion()
        {

            this._TipoOrigenId = ConstantesModelo.IDORIGEN_ORGANIZACION;
        }


        /// <summary>
        /// Nombre único del cuadro de clasificación
        /// </summary>
        public string Nombre { get ; set ; }

        /// <summary>
        /// Especifica si el elemento ha sido marcado como eliminado
        /// </summary>
        public bool Eliminada { get; set; }


        private string _TipoOrigenId;
        /// <summary>
        /// El tipo de orígen en para este modelo es la unidad organizacional a la que pertenece el cuadro de clasificacion
        /// </summary>
        public string TipoOrigenId {
            get { return _TipoOrigenId; }
            set { 
            // el valor no debe cambiarse
            }
        }
    

        /// <summary>
        /// Identificador de la organización a la que pertenece el cuadro de clasificación
        /// </summary>
        public string OrigenId { get; set; }


        /// <summary>
        /// Identificaidor del estado del cuadro de clasificación
        /// </summary>
        public string EstadoCuadroClasificacionId { get; set; }


        /// <summary>
        /// Esatdo del cuadro de clasificación
        /// </summary>
        public virtual EstadoCuadroClasificacion Estado { get; set; }

    }
}
