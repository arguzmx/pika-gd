using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ListAttribute : Attribute
    {

        /// <summary>
        /// Otiene el atributo de lista
        /// </summary>
        /// <param name="Entidad"></param>
        /// <param name="DatosRemotos"></param>
        /// <param name="TypeAhead"></param>
        /// <param name="OrdenarAlfabetico"></param>
        /// <param name="Default">Valor por default de la lista</param>
        /// <param name="ValoresCSV">Lista de valores separados por comas</param>
        /// <param name="EsListaTemas">Especific si la lsita es un tema de seeción de la entidad</param>
        /// <param name="FiltroBusqueda">Determina si require un filtro para la búsqueada de pares para obenerlo desde el servivdor</param>
        public ListAttribute(string Entidad, bool DatosRemotos = false,  
            bool TypeAhead=false, bool OrdenarAlfabetico = false, string Default = "", string ValoresCSV="",
            bool EsListaTemas = false, bool FiltroBusqueda = false) {
            this._Entidad = Entidad;
            this._DatosRemotos = DatosRemotos;
            this._OrdenarAlfabetico = OrdenarAlfabetico;
            this._TypeAhead = TypeAhead;
            this._Default = Default;
            this._ValoresCSV = ValoresCSV;
            this._EsListaTemas = EsListaTemas;
            _FiltroBusqueda = FiltroBusqueda;
        }


        private bool _FiltroBusqueda;
        private bool _EsListaTemas;
        private string _Default;
        private string _ValoresCSV;

        public virtual bool EsListaTemas
        {
            get { return _EsListaTemas; }
        }

        public virtual bool FiltroBusqueda
        {
            get { return _FiltroBusqueda; }
        }

        public virtual string ValoresCSV
        {
            get { return _ValoresCSV; }
        }

        /// <summary>
        /// Nombre el valor default para la list
        /// </summary>
        public virtual string Default
        {
            get { return _Default; }
        }

        private string _Entidad;

        /// <summary>
        /// Nombre de la entidad necesarioa 
        /// </summary>
        public virtual string Entidad
        {
            get { return _Entidad; }
        }

        private bool _DatosRemotos;
        /// <summary>
        /// Obtiene los datos desde el endpoint cuando se encuentra habilitado
        /// </summary>
        public  virtual bool DatosRemotos { get => _DatosRemotos; }

        private bool _TypeAhead;
        /// <summary>
        /// Especifica si los datos remotos se obtienen en la modalidad TypeAhead
        /// En caso contrario se otiene la totalidad de los elementos
        /// </summary>
        public bool TypeAhead { get => _TypeAhead; }




        private bool _OrdenarAlfabetico;
        /// <summary>
        /// Idica si la lsita debe orndearse alfabéticmaente en caso contrario
        /// se intentará ordenar en base a la propiedad indice y si no existe en el 
        /// orden por defecto de la entrega de dtos
        /// </summary>
        public bool OrdenarAlfabetico { get => _OrdenarAlfabetico; }

    }
}
