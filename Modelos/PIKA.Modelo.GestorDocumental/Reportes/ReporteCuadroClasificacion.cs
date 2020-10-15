﻿using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes
{
    public class ReporteCuadroClasificacion : IProveedorReporte
    {
        private const string nombre = "Cuadro de clasificación";
        private const string url = "reporte/cuadro/{id}";
        private const string id = "cuadrodeclasificacion";

        private List<ParametroReporte> parametroes ;
        private List<FormatoReporte> formatos;
        
        public ReporteCuadroClasificacion()
        {
            parametroes = new List<ParametroReporte>();
            formatos = new List<FormatoReporte>();

            parametroes.Add(new ParametroReporte() { Id = "id", 
                Nombre = "Identificador único cuadro clasifiacion", 
                Tipo = TipoDato.tString , 
                Contextual = true,
                IdContextual = ConstantesModelo.PREFIJO_CONEXTO + "Id"
            });

            
            formatos.Add(new FormatoReporte() { Id = FormatoReporte.EXCEL, Nombre = "Excel" });

            this.Nombre = nombre;
            this.Url = url;
            this.Id = id;
            this.DatosJson = false;
            this.Parametros = parametroes;
            this.FormatosDisponibles = formatos;

        }

        public bool DatosJson { get; set; }

        public string Id { get; set; }

        public string Nombre { get; set; }
        public string Url { get ; set; }

        public List<ParametroReporte> Parametros { get; set; }


        public List<FormatoReporte> FormatosDisponibles { get; set; }
    }
}
