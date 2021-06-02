using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class Consulta : ParametrosConsulta
    {
        public Consulta()
        {
            this.Filtros = new List<FiltroConsulta>();
        }

        public List<FiltroConsulta> Filtros { get; set; }
        public string IdCache { get; set; }

    }

    public class ConsultaAPI
    {
        public ConsultaAPI()
        {

            this.Filtros = new List<FiltroConsulta>();
            this.Ids = new List<string>();
        }

        /// <summary>
        /// Indíce en base cero de la página solicitada 
        /// </summary>
        public int indice { get; set; }

        /// <summary>
        /// Tamaño de la página solicitada
        /// </summary>
        public int tamano { get; set; }

        /// <summary>
        /// Número consecytivo paradar seguimiento en llamdas asínctonas
        /// </summary>
        public int consecutivo { get; set; }

        /// <summary>
        /// Nombre de la columna de ordenamoento
        /// </summary>
        public string ord_columna { get; set; }
        public string ord_direccion { get; set; }

        /// <summary>
        /// Especifica si los totales de paginado deben recalcularse
        /// </summary>
        public bool recalcular_totales { get; set; }
        public List<FiltroConsulta> Filtros { get; set; }

        public List<string> Ids { get; set; }
        public string IdCache { get; set; }

        public Consulta AConulta()
        {
            return new Consulta()
            {
                consecutivo = this.consecutivo,
                indice = this.indice,
                Filtros = this.Filtros,
                ord_columna = this.ord_columna,
                ord_direccion = this.ord_direccion,
                recalcular_totales = this.recalcular_totales,
                tamano = this.tamano
            };
        }

    }

}
