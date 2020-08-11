using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    

    /// <summary>
    /// Permite deifinir un vínculo de navegación entre entidades
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class EntidadVinculadaAttribute : Attribute
    {
    
        private TipoCardinalidad _Cardinalidad;
        private string _Entidad;
        private string _Padre;
        private string _Hijo;

        private TipoDespliegueVinculo _TipoDespliegueVinculo;

        public EntidadVinculadaAttribute(string EntidadHijo = ""
        , TipoCardinalidad Cardinalidad = TipoCardinalidad.UnoVarios
        , string PropiedadPadre ="", string PropiedadHijo = "", TipoDespliegueVinculo TipoDespliegueVinculo = TipoDespliegueVinculo.Tabular)
        {
            this._Cardinalidad = Cardinalidad;
            this._Entidad = EntidadHijo;
            this._Padre = PropiedadPadre;
            this._Hijo = PropiedadHijo;
            this._TipoDespliegueVinculo = TipoDespliegueVinculo;
        }

        /// <summary>
        /// Cardinalida de la relación expresada de padre a hijo
        /// </summary>
        public virtual TipoCardinalidad Cardinalidad
        {
            get { return _Cardinalidad; }
        }

        /// <summary>
        /// NOmbre de la entida hijo en la relación
        /// </summary>
        public virtual string Entidad
        {
            get { return _Entidad; }
        }

        /// <summary>
        /// NOm,bre de la propiedad de relación en el padre
        /// </summary>
        public virtual string PropiedadPadre
        {
            get { return _Padre; }
        }

        // Nombre de la propiedad de relación en la entidad hiji
        public virtual string PropiedadHijo
        {
            get { return _Hijo; }
        }


        public virtual TipoDespliegueVinculo TipoDespliegue
        {
            get { return _TipoDespliegueVinculo; }
        }

    }
}
