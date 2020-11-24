using Nest;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Instancias
{

    [ElasticsearchType(RelationName = "VinculosObjetoPlantilla", IdProperty = "_Id" )]
    public class VinculosObjetoPlantilla
    {

        public VinculosObjetoPlantilla()
        {
            Documentos = new List<VinculoDocumentoPlantilla>();
            Listas = new List<VinculoListaPlantilla>();
        }

        public string _Id { get; set; }

        /// <summary>
        /// Identificador del tipo de objeto al que esta asociadao la plantilla
        /// </summary>
        [Keyword]
        public string Tipo { get; set; }


        /// <summary>
        /// Identificador único del objeto asociado a la planttila
        /// </summary>
        [Keyword]
        public string Id { get; set; }


        /// <summary>
        /// Documentos vinculaods uno a uno con el objeto
        /// </summary>
        [Nested]
        [PropertyName("Documentos")]
        public List<VinculoDocumentoPlantilla> Documentos { get; set; }


        /// <summary>
        /// Listas de documento asociadas uno a vrios con el documenot
        /// </summary>
        [Nested]
        [PropertyName("Listas")]
        public List<VinculoListaPlantilla> Listas { get; set; }


    }
}
