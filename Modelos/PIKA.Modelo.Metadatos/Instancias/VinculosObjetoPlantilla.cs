using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Instancias
{
    public class VinculosObjetoPlantilla: IEntidadRelacionada
    {

        public VinculosObjetoPlantilla()
        {
            Documentos = new List<VinculoDocumentoPlantilla>();
            Listas = new List<VinculoListaPlantilla>();
        }

        /// <summary>
        /// Identificador´del tipo de objeto al que esta asociadao la plantilla
        /// </summary>
        public string TipoOrigenId { get; set; }


        /// <summary>
        /// Identificador único del objeto asociado a la planttila
        /// </summary>
        public string OrigenId { get; set; }


        /// <summary>
        /// Documentos vinculaods uno a uno con el objeto
        /// </summary>
        public List<VinculoDocumentoPlantilla> Documentos { get; set; }


        /// <summary>
        /// Listas de documento asociadas uno a vrios con el documenot
        /// </summary>
        public List<VinculoListaPlantilla> Listas { get; set; }

        public string TipoOrigenDefault => "";


    }
}
