using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class FormatoReporte: EntidadCatalogo<string, FormatoReporte>
    {

        public const string PDF = "pdf";
        public const string EXCEL = "xlsx";
        public const string WORD = "docx";
        public const string CSV = "csv";

        public override string Id { get => base.Id; set => base.Id = value; }

        public override List<FormatoReporte> Seed()
        {
            List<FormatoReporte> lista = new List<FormatoReporte>();

            lista.Add(new FormatoReporte() { Id = PDF, Nombre = "PDF" });
            lista.Add(new FormatoReporte() { Id = EXCEL, Nombre = "Excel" });
            lista.Add(new FormatoReporte() { Id = WORD, Nombre = "Word" });
            lista.Add(new FormatoReporte() { Id = CSV, Nombre = "Valores separados por comas" });

            return lista;
        }

    }
}
