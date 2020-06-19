using Microsoft.CodeAnalysis.CSharp.Syntax;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PIKA.Modelo.Metadatos.Atributos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ValuelistAttribute: Attribute
    {
        /// <summary>
        /// Los datos vienen de un endpointde api standar API del sistema
        /// </summary>
        public const string ORIGEN_API = "api";

        /// <summary>
        /// Los datos vienen de un arreglo constante establecido en el código
        /// u obtenido de un almacenamiento tal como base de datos
        /// </summary>
        public const string ORIGEN_STORE = "store";



        private string _origen;
        private string _entidadAPi;
        private bool _typeahead;

        public ValuelistAttribute(string Origen = ORIGEN_STORE, string EntidadAPI="", bool TypeAhead=false)
        {
            _origen = Origen;
            _entidadAPi = EntidadAPI;
            _typeahead = TypeAhead;

        }
       

        /// <summary>
        /// Origen de los datos para la lista de valores
        /// utilizar contantes OEIGN de esta clase
        /// </summary>
        public string Origen { get { return _origen; } }

        /// <summary>
        /// Nombre del tipo de entidad para la consulta API
        /// </summary>
        public string EntidadAPI { get { return _entidadAPi;  } }


        /// <summary>
        /// Nombre del tipo de entidad para la consulta API
        /// </summary>
        public bool TypeAhead { get { return _typeahead; } }


    }
}
