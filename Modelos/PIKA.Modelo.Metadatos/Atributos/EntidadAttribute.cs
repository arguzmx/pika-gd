using System;
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

        public EntidadAttribute(bool EliminarLogico = false, string Columna = "Eliminada")
        {
            this._Columna = EliminarLogico ? Columna :"";
            this._EliminarLogico = EliminarLogico;
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


    }
}
