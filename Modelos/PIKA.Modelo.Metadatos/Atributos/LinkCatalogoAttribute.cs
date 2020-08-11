using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Atributos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class LinkCatalogoAttribute: Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EntidadCatalogo">Entidad catalogo genralemnte hereda de IEntidadCatalogo</param>
        /// <param name="IdCatalogo">Columna Id del catálogo</param>
        /// <param name="IdEntidad">Columna Id de la entidad</param>
        /// <param name="IdCatalogoMap">Columna en la entidad vinculante para el catalogo</param>
        /// <param name="IdEntidadMap">Columna en la entidad vinculante para la entidado</param>
        /// <param name="EntidadVinculo">Entidad vinculante</param>
        /// <param name="PropiedadReceptora">Nombre de la proiedad del tipo array que recibe lso valores en la API</param>
        /// <param name="Despliegue">Tipo de despliegue para la captura</param>
        public LinkCatalogoAttribute(string EntidadCatalogo, 
            string IdCatalogo, string IdEntidad,
            string IdCatalogoMap, string IdEntidadMap,
            string EntidadVinculo, string PropiedadReceptora, TipoDespliegueVinculo Despliegue = TipoDespliegueVinculo.GrupoCheckbox)
        {
            _EntidadCatalogo = EntidadCatalogo;
            _IdCatalogo = IdCatalogo;
            _IdEntidad = IdEntidad;
            _Despliegue = Despliegue;
            _EntidadVinculo = EntidadVinculo;
            _IdCatalogoMap = IdCatalogoMap;
            _IdEntidadMap = IdEntidadMap;
            _PropiedadReceptora = PropiedadReceptora;
        }

        private string _PropiedadReceptora;
        public virtual string PropiedadReceptora
        {
            get { return _PropiedadReceptora; }
        }

        private string _EntidadVinculo;
        public virtual string EntidadVinculo
        {
            get { return _EntidadVinculo; }
        }

        private string _EntidadCatalogo;
        public virtual string EntidadCatalogo
        {
            get { return _EntidadCatalogo; }
        }

        private string _IdCatalogo;
        public virtual string IdCatalogo
        {
            get { return _IdCatalogo; }
        }

        private string _IdEntidad;
        public virtual string IdEntidad
        {
            get { return _IdEntidad; }
        }

        private string _IdCatalogoMap;
        public virtual string IdCatalogoMap
        {
            get { return _IdCatalogoMap; }
        }

        private string _IdEntidadMap;
        public virtual string IdEntidadMap
        {
            get { return _IdEntidadMap; }
        }

        private TipoDespliegueVinculo _Despliegue;
        public virtual TipoDespliegueVinculo Despliegue
        {
            get { return _Despliegue; }
        }

    }
}
