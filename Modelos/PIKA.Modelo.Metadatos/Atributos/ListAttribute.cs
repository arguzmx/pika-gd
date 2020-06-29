using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ListAttribute : Attribute
    {


        public ListAttribute(string Entidad, bool DatosRemotos = false, 
            bool TypeAhead=false, bool OrdenarAlfabetico = false) {
            
            this._Entidad = Entidad;
            this._DatosRemotos = DatosRemotos;
            this._OrdenarAlfabetico = OrdenarAlfabetico;
            this._TypeAhead = TypeAhead;
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
