﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Metadatos
{


    /// <summary>
    /// Mantiene una copia de los datos corespondientes a una plantilla
    /// Esta clase es solo para manejo en memodia no debe incluirse en la base de datos
    /// </summary>
    public class DocumentoPlantilla: IEntidadRelacionada
    {
        public DocumentoPlantilla() {
            this.Valores = new List<ValorPropiedad>();
        }

        public DocumentoPlantilla(string plantillaId)
        {
            this.PlantillaId = plantillaId;
            this.Valores = new List<ValorPropiedad>();
        }

        /// <summary>
        /// Identificador único del registro de metadatos
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Identificador único de la plantilla
        /// </summary>
        [Required]
        [MinLength(1)]
        public string PlantillaId { get; set; }

        /// <summary>
        /// Lista de valores en las propiedades
        /// </summary>
        public List<ValorPropiedad> Valores { get; set; }

        /// <summary>
        /// Identifica si el documento forma parte de una lista
        /// </summary>
        public bool EsLista { get; set; }

        /// <summary>
        /// Almacena el Id Unico de la lista  la que pertenece el documento
        /// </summary>
        public string ListaId { get; set; }

        /// <summary>
        /// Dominio de los datos
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Uniodad organizacional o tenant
        /// </summary>
        public string OrigenId { get; set; }


        /// <summary>
        /// Identificador del tipo de datp asoaciado a los valores, por ejemplo Dominio, Persona, Documentp
        /// </summary>
        [Required]
        public string TipoDatoId { get; set; }

        /// <summary>
        /// Identificador único de la instancia datos a la que se asocian los valores
        /// por ejemplo Usuario: Oswaldo Diaz, Documento: Carta factura 1o Febrero
        /// </summary>
        [Required]
        public string DatoId { get; set; }

        /// <summary>
        /// Información adicional para filtrado, por ejemplo el volumen en 
        /// el caseo del contedido, podría de la concatenación pue permite
        /// identificar univocamente los datos del objeto asociado dentro de 
        /// un universo de valores
        /// </summary>
        public string IndiceFiltrado { get; set; }

        /// <summary>
        /// Est indice permite filtrar cuando un elemento se encuentre asociado
        /// a una entidad jerárquica padre tal como carpetas en el contenido
        /// </summary>
        public string IndiceJerarquia { get; set; }

        /// <summary>
        /// Propiedad no utilizada
        /// </summary>
        public string TipoOrigenDefault => "";

    }



}
