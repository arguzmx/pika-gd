using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class TipoDato : EntidadCatalogo<string, TipoDato>
    {
        public const string tString = "string";
        public const string tDouble = "double";
        public const string tBoolean = "bool";
        public const string tInt32 = "int";
        public const string tInt64 = "long";
        public const string tDateTime = "datetime";
        public const string tDate = "date";
        public const string tTime = "time";
        public const string tBinaryData = "bin";
        public const string tList = "list";


        public override List<TipoDato> Seed()
        {
            List<TipoDato> l = new List<TipoDato>();

            l.Add(new TipoDato() { Id = tString, Nombre = "Texto" });
            l.Add(new TipoDato() { Id = tDouble, Nombre = "Número decimal" });
            l.Add(new TipoDato() { Id = tBoolean, Nombre = "Booleano" });
            l.Add(new TipoDato() { Id = tInt32, Nombre = "Entero" });
            l.Add(new TipoDato() { Id = tInt64, Nombre = "Entero largo" });
            l.Add(new TipoDato() { Id = tDateTime, Nombre = "Fecha y hora" });
            l.Add(new TipoDato() { Id = tDate, Nombre = "Fecha" });
            l.Add(new TipoDato() { Id = tTime, Nombre = "Hora" });
            l.Add(new TipoDato() { Id = tList, Nombre = "Lista valores" });
            l.Add(new TipoDato() { Id = tBinaryData, Nombre = "Datos binarios" });

            return l.OrderBy(x=>x.Nombre).ToList();

        }

        /// <summary>
        /// Propedad de navegación
        /// </summary>
        public virtual ICollection<TipoDatoPropiedadPlantilla> PropiedadesPlantilla { get; set; }

    }
}
