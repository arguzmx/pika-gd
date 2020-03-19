using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Infraestructura.UI
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class UIJerarquiaAttribute: Attribute 
    {
        private string _Tipo;
        /// <summary>
        /// Nombre completo del tipo en el ensamblado
        /// </summary>
        public virtual string Tipo
        {
            get { return _Tipo; }
        }

       private string _PropidadAgrupador;
        /// <summary>
        /// Nombre de la propiedade utilizada como agrupador para la consulta
        /// </summary>
        public virtual string PropidadAgrupador
        {
            get { return _PropidadAgrupador; }
        }


        private string _PropiedadPadreId;
        /// <summary>
        /// Nombre de la propiedade utilizada como Id padre
        /// </summary>
        public virtual string PropiedadPadreId
        {
            get { return _PropiedadPadreId; }
        }


        private string _PropiedadTexto;
        /// <summary>
        /// Nombre de la propiedade utilizada como texto
        /// </summary>
        public virtual string PropiedadTexto
        {
            get { return _PropiedadTexto; }
        }


        private string _PropiedadId;
        /// <summary>
        /// Nombre de la propiedade utilizada como Id
        /// </summary>
        public virtual string PropiedadId
        {
            get { return _PropiedadId; }
        }


        private string _Endpoint;
        /// <summary>
        /// Ruta de la API
        /// </summary>
        public virtual string Endpoint
        {
            get { return _Endpoint; }
        }
        /// <summary>
        /// Crea una isntancia del atributo
        /// </summary>
        /// <param name="Tipo">Nombre completo del tipo en el ensamblado</param>
        /// <param name="Id">Nombre de la propiedade utilizada como Id</param>
        /// <param name="Texto">Nombre de la propiedade utilizada como texto</param>
        /// <param name="PadreId">Nombre de la propiedade utilizada como  Id padre</param>
        /// <param name="Agruapdor">Nombre de la propiedade utilizada como  Agruapdor para la consulta</param>
        public UIJerarquiaAttribute(string Tipo, string Id, string Texto, string PadreId, string Endpoint, string Agruapdor="" ) {

            _Tipo = Tipo;
            _PropiedadId = Id;
            _PropiedadTexto = Texto;
            _PropiedadPadreId = PadreId;
            _PropidadAgrupador = Agruapdor;
            _Endpoint = Endpoint;

        }

    }
}
