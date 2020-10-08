using Microsoft.AspNetCore.Mvc.RazorPages;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private string _PropiedadIdMiembro;
        private string _TokenSeguridad;
        private bool _HijoDinamico;
        private TipoDespliegueVinculo _TipoDespliegueVinculo;
        private List<DiccionarioEntidadVinculada> _DiccionarioEntidadVinculada;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="TokenSeguridad">Token para validar acceso</param>
        /// <param name="EntidadHijo"></param>
        /// <param name="Cardinalidad"></param>
        /// <param name="PropiedadPadre"></param>
        /// <param name="PropiedadHijo"></param>
        /// <param name="PropiedadIdMiembro"> Nombre de la propiedad Id de los miembros cuando el tipo de vlculo es membresia</param>
        /// <param name="TipoDespliegueVinculo">Tipo de despliegue</param>
        /// <param name="HijoDinamico">Determina si el hijo de vinculación es dinámico en base a la propeieda del objecto definida por EntidadHijo</param>
        /// <param name="Diccionario">El diccionario se forma de pres seprardos por , separados por |</param>
        public EntidadVinculadaAttribute(string TokenSeguridad, string EntidadHijo = ""
        , TipoCardinalidad Cardinalidad = TipoCardinalidad.UnoVarios
        , string PropiedadPadre = "", string PropiedadHijo = ""
        , TipoDespliegueVinculo TipoDespliegueVinculo = TipoDespliegueVinculo.Tabular, 
            bool HijoDinamico = false, string PropiedadIdMiembro="", string Diccionario = "")
        {
            _DiccionarioEntidadVinculada = new List<DiccionarioEntidadVinculada>();
            if ( !string.IsNullOrEmpty(Diccionario))
            {
                List<string> pares = Diccionario.Split('|').ToList();
                foreach(string s in pares)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        List<string> Par = s.Split(',').ToList();
                        if( Par.Count == 2)
                        {
                            this._DiccionarioEntidadVinculada.Add(new DiccionarioEntidadVinculada()
                            {
                                Enidad = Par[1],
                                Id = Par[0]
                            });
                        }
                    }
                    
                }
            }
            this._TokenSeguridad = TokenSeguridad;
            this._PropiedadIdMiembro = PropiedadIdMiembro;
            this._Cardinalidad = Cardinalidad;
            this._Entidad = EntidadHijo;
            this._Padre = PropiedadPadre;
            this._Hijo = PropiedadHijo;
            this._TipoDespliegueVinculo = TipoDespliegueVinculo;
            this._HijoDinamico = HijoDinamico;
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

        /// <summary>
        /// Token de validación para acceso  a la vista
        /// </summary>
        public virtual string TokenSeguridad
        {
            get { return _Padre; }
        }

        // Nombre de la propiedad de relación en la entidad hiji
        public virtual string PropiedadHijo
        {
            get { return _Hijo; }
        }


        // Nombre de la propiedad Id de los miembros cuando el tipo de vlculo es membresia
        public virtual string PropiedadIdMiembro
        {
            get { return _PropiedadIdMiembro; }
        }

        public virtual TipoDespliegueVinculo TipoDespliegue
        {
            get { return _TipoDespliegueVinculo; }
        }

        //Detrminas si el hijo del vínculo se define en base a la propiedad definica por 
        //el valor del campo Entidad
        public virtual bool HijoDinamico
        {
            get { return _HijoDinamico; }
        }

        public virtual List<DiccionarioEntidadVinculada> DiccionarioEntidadesVinculadas
        {
            get { return _DiccionarioEntidadVinculada; }
        }

    }
}
