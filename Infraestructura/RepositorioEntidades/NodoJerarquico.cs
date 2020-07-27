using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class NodoJerarquico : Entidad<string>
    {
        /// <summary>
        /// Identificador único del elemento de lista
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Testo del elemento de lista
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Indice para ordenar los elemento de la lista
        /// </summary>
        public int Indice { get; set; }

        public List<NodoJerarquico> Hijos { get; set; }

    }
}
