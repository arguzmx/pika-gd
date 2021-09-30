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
        private string _TokenMod;
        private string _TokenApp;
        private bool _AsociadoMetadatos;
        private bool _HabilitarSeleccion;
        private bool _PermiteAltas;
        private bool _PermiteBajas;
        private bool _PermiteCambios;

        public EntidadAttribute( bool PaginadoRelacional=false, 
            bool EliminarLogico = false, 
            bool OpcionActivar = false,
           string ColumnaActivar = "", 
           string Columna = "Eliminada", 
           string TokenMod = "", 
           string TokenApp = "", 
           bool AsociadoMetadatos = false,
           bool HabilitarSeleccion = false,
           bool PermiteAltas = true,
           bool PermiteBajas = true,
           bool PermiteCambios = true)
        {
            this._Columna = EliminarLogico ? Columna :"";
            this._EliminarLogico = EliminarLogico;
            this._PaginadoRelacional = PaginadoRelacional;
            this._ColumnaActivar = ColumnaActivar;
            this._OpcionActivar = OpcionActivar;
            _TokenApp = TokenApp;
            _TokenMod = TokenMod;
            _AsociadoMetadatos = AsociadoMetadatos;
            _HabilitarSeleccion = HabilitarSeleccion;
            _PermiteAltas = PermiteAltas;
            _PermiteBajas = PermiteBajas;
            _PermiteCambios = PermiteCambios;

        }


        public virtual bool PermiteCambios
        {
            get { return _PermiteCambios; }
        }

        public virtual bool PermiteBajas
        {
            get { return _PermiteBajas; }
        }

        public virtual bool PermiteAltas
        {
            get { return _PermiteAltas; }
        }


        /// <summary>
        /// Determina si puede crearse una selección de las entidades para el usuario en sesión
        /// </summary>
        public virtual bool HabilitarSeleccion
        {
            get { return _HabilitarSeleccion; }
        }


        /// <summary>
        /// detrmina si la entidas puede asociarse a los metadtaos
        /// </summary>
        public virtual bool AsociadoMetadatos
        {
            get { return _AsociadoMetadatos; }
        }

        public virtual string TokenMod
        {
            get { return _TokenMod; }
        }

        public virtual string TokenApp
        {
            get { return _TokenApp; }
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
