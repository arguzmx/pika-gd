using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Busqueda
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ContextoBusqedaAttribute : Attribute
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">Identificador único del tipo de busqueda</param>
        /// <param name="Contexto">Especifica el contexto de la aplicación para el agrupamiento de servicios de búsqueda</param>
        public ContextoBusqedaAttribute(string Id, string Contexto)
        {
            this._Contexto = Contexto;
            this._Id = Id;
        }

        private string _Contexto { get; set; }
        public virtual string Contexto
        {
            get { return _Contexto; }
        }

        private string _Id { get; set; }
        public virtual string Id
        {
            get { return _Id; }
        }
    }

}
