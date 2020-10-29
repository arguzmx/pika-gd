﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

    /// <summary>
    /// Mantiene una copia de los datos corespondientes a una plantilla
    /// Esta clase es solo para manejo en memodia no debe incluirse en la base de datos
    /// </summary>
    public class ValoresPlantilla: IEntidadRelacionada
    {
        public ValoresPlantilla() {
            this.Valores = new List<ValorPropiedad>();
        }

        public ValoresPlantilla(string plantillaId)
        {
            this.PlantillaId = plantillaId;
            this.Valores = new List<ValorPropiedad>();
        }

        /// <summary>
        /// Identificador único del registro de metadatos
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Identifica si el registro es único para la insercion
        /// </summary>
        public bool Unico { get; set; }


        /// <summary>
        /// Identificador único de la plantilla
        /// </summary>
        public string PlantillaId { get; set; }

        /// <summary>
        /// Lista de valores en las propiedades
        /// </summary>
        public List<ValorPropiedad> Valores { get; set; }

        /// <summary>
        /// Propiedad no utilizada
        /// </summary>
        public string TipoOrigenDefault => "";


        /// <summary>
        /// Identificador del tipo de origen asoaciado a los valores, por ejemplo Dominio, Persona, Documentp
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador único de la instancia origen a la que se asocian los valores
        /// por ejemplo Usuario: Oswaldo Diaz, Documento: Carta factura 1o Febrero
        /// </summary>
        public string OrigenId { get; set; }


        /// <summary>
        /// Inserta un valor en la lista de valores para la plantilla
        /// </summary>
        /// <param name="PropiedadId">Identificador único de la propiedad</param>
        /// <param name="Valor">Valor asociado a la propiedad</param>
        public void  InsertaValor(string PropiedadId, string Valor)
        {
            if (string.IsNullOrEmpty(PropiedadId)) throw new Exception("El id de la propiedad es requerido");

            if (Valores.Where(x => x.PropiedadId == PropiedadId).Any()) {
                Valores.Where(x => x.PropiedadId == PropiedadId).First().Valor = Valor;
            } else
            {
                Valores.Add(new ValorPropiedad() { PropiedadId = PropiedadId, Valor = Valor });
            }
        }

        
        
        

    }

    

}
