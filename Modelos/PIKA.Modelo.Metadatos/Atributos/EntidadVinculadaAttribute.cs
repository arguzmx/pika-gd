using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public enum TipoCardinalidad
    {
        UnoVarios = 0, UnoUno = 1
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class EntidadVinculadaAttribute : Attribute
    {
    
        private TipoCardinalidad _Cardinalidad;
        private string _Entidad;
        private string _Padre;
        private string _Hijo;

        public EntidadVinculadaAttribute(string Entidad = ""
        , TipoCardinalidad Cardinalidad = TipoCardinalidad.UnoVarios
        , string Padre ="", string Hijo = "")
        {
            this._Cardinalidad = Cardinalidad;
            this._Entidad = Entidad;
            this._Padre = Padre;
            this._Hijo = Hijo;
        }

        public virtual TipoCardinalidad Cardinalidad
        {
            get { return _Cardinalidad; }
        }

        public virtual string Entidad
        {
            get { return _Entidad; }
        }


        public virtual string Padre
        {
            get { return _Padre; }
        }

        public virtual string Hijo
        {
            get { return _Hijo; }
        }

    }
}
