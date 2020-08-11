using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class PropAttribute : Attribute
    {

        private bool _Required;
        private string _Id;
        private string _Type;
        private int _OrderIndex;
        private bool _Searchable;
        private bool _Orderable;
        private bool _isId;
        private bool _Visible;
        private bool _IsLabel;
        private bool _HieId;
        private bool _HieRoot;
        private bool _HieName;
        private bool _HieParent;
        private bool _ShowInTable;
        private bool _Contextual;
        private string _IdContextual;
        private bool _ToggleInTable;
        private string _DefaultValue;
        private int _TableOrderIndex;

        public PropAttribute(string Type = "",
            [CallerMemberName] string Id = null, string DefaultValue = "",
            int OrderIndex = 0, bool Searchable = true, bool Orderable = true, bool Visible = true,
            bool Required = false, bool isId = false, bool ShowInTable = true, bool ToggleInTable = true,
            int TableOrderIndex = 0, bool Contextual = false, string IdContextual = "" , bool IsLabel = false,
            bool HieId = false, bool HieName = false, bool HieRoot = false, bool HieParent = false)
        {

            this._Contextual = Contextual;
            this._TableOrderIndex = (TableOrderIndex == 0) ? OrderIndex : TableOrderIndex;
            this._ShowInTable = ShowInTable;
            if(!ShowInTable)
            {
                this._ToggleInTable = false;
            } else
            {
                this._ToggleInTable = ToggleInTable;
            }
            
            this._Required = Required;
            this._Visible = Visible;
            this._Orderable = Orderable;
            this._Searchable = Searchable;
            this._Type = Type;
            this._Id = Id;
            this._OrderIndex = OrderIndex;
            this._DefaultValue = DefaultValue;
            this._isId = isId;
            this._IsLabel = IsLabel;
            this._HieName = HieName;
            this._HieRoot = HieRoot;
            this._HieParent = HieParent;
            this._HieId = HieId;
            if(this.Contextual)
            {
                this._IdContextual = IdContextual;
            }

        }

        /// <summary>
        /// Especifica si la propieda debe mostrarse ne la tabla de detalle
        /// </summary>
        public virtual bool ShowInTable
        {
            get { return _ShowInTable; }
        }


        /// <summary>
        ///  Indica si una proidad de obtiene del contexto de ejecución
        /// </summary>
        public virtual bool Contextual
        {
            get { return _Contextual; }
        }

        /// <summary>
        /// Identificadorúnico de la variable contextual para el mapeo
        /// </summary>
        public virtual string IdContextual
        {
            get { return _IdContextual; }
        }

        /// <summary>
        /// Especifica si la propiedad al formar parte de una tabla puede alternar su visibilidad
        /// </summary>
        public virtual bool ToggleInTable
        {
            get { return _ToggleInTable; }
        }


        /// <summary>
        /// Especifica el orden de despliegue en la tabla
        /// </summary>
        public virtual int TableOrderIndex
        {
            get { return _TableOrderIndex; }
        }


        /// <summary>
        /// Especifica si la propiuedad es un identificador del objeto,
        /// estos campos deben ser incluids durante las operaciones de actualización
        /// </summary>
        public virtual bool isId
        {
            get { return _isId; }
        }


        /// <summary>
        /// Determina si el contenido de l apropiedad puede utilizaerse como etiqueta de despliegie
        /// para humanos
        /// </summary>
        public virtual bool isLabel
        {
            get { return _IsLabel; }
        }


        /// <summary>
        /// Especifica la propeidad es requerida
        /// </summary>
        public virtual bool Required
        {
            get { return _Required; }
        }

        /// <summary>
        /// Inndice para el despliegue de la propiedad
        /// </summary>
        public virtual int OrderIndex
        {
            get { return _OrderIndex; }
        }

        /// <summary>
        /// String tipo de la propiedad de acuerdo a la entidad TIPODATO
        /// </summary>
        public virtual string Type
        {
            get { return _Type; }
        }

        /// <summary>
        /// Especifica si la propiedad puede ser utilziada para ordenar un arreglo de entidades
        /// </summary>
        public virtual bool Orderable
        {
            get { return _Orderable; }
        }

        /// <summary>
        /// Especifica si la propiedad puede ser utilziada para buscar en arreglo de entidades
        /// </summary>
        public virtual bool Searchable
        {
            get { return _Searchable; }
        }


        /// <summary>
        /// Especifica si la propiedad es visible
        /// </summary>
        public virtual bool Visible
        {
            get { return _Visible; }
        }

        /// <summary>
        /// Contiene el texto utilizado como nombre de la propiedad en el idiioma de la UI
        /// y cooresponde al Id de la misma
        /// </summary>
        public virtual string Id
        {
            get { return _Id; }
        }

        /// <summary>
        /// Especifica el varlo defualt para la propiedad
        /// </summary>
        public virtual string DefaultValue
        {
            get { return _DefaultValue; }
        }

        public virtual bool HieId
        {
            get { return _HieId; }
        }

        /// <summary>
        /// Especifica si la propiedad es el identiicador padre para una jerarquia de entidades
        /// </summary>
        public virtual bool HieRoot
        {
            get { return _HieRoot; }
        }


        /// <summary>
        /// Especifica si la propiedad es la etiqueta para una jerarquía de entidades
        /// </summary>
        public virtual bool HIeName
        {
            get { return _HieName; }
        }

        /// <summary>
        /// Determina si el nodo se utiliza como padre en las jerarquias
        /// </summary>
        public virtual bool HieParent
        {
            get { return _HieParent; }
        }


    }
}
