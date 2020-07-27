﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Agrega propiedadesgenerales sobre la entidad 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class EntidadAttribute : Attribute
    {

        private bool _EliminarLogico;
        private string _Columna;
        private bool _PaginadoRelacional;
        private string _ColumnaActivar;
        private bool _OpcionActivar;

        public EntidadAttribute(bool PaginadoRelacional=false, bool EliminarLogico = false, bool OpcionActivar = false,
           string ColumnaActivar = "", string Columna = "Eliminada")
        {
            this._Columna = EliminarLogico ? Columna :"";
            this._EliminarLogico = EliminarLogico;
            this._PaginadoRelacional = PaginadoRelacional;
            this._ColumnaActivar = ColumnaActivar;
            this._OpcionActivar = OpcionActivar;
        }

        /// <summary>
        /// Especifica si la entidad requuiere paginado relacionar utlizando el TipoOrigen y el Id del origen
        /// </summary>
        public virtual bool PaginadoRelacional
        {
            get { return _PaginadoRelacional; }
        }

        /// <summary>
        /// Indica si la entidad se elimna de manera lógica o la eliminaicón es permanente
        /// </summary>
        public virtual bool EliminarLogico
        {
            get { return _EliminarLogico; }
        }

        /// <summary>
        /// Nombre de la columna utilizada para marcar el eliminado lógic
        /// Esto es de interes para la UI por ejemplo para crear un filtro por defecto 
        /// para mostrar solo las no eliminadas
        /// </summary>
        public virtual string Columna
        {
            get { return _Columna; }
        }

        /// <summary>
        /// Determina si la entidad tiene un estado que permit amarcarla como activa/inactiva
        /// </summary>
      public virtual bool OpcionActivar
        {
            get { return _OpcionActivar; }
        }

        /// <summary>
        /// Especifica si la entidad permite los estados activo/inactivo a atrvés de la definicón deuna columna 
        /// logica que almacene esta información
        /// </summary>
        public virtual string ColumnaActivar
        {
            get { return _ColumnaActivar; }
        }

    }
}
