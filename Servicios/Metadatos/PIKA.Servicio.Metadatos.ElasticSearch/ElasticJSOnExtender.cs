using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nest;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public static class ElasticJSONExtender
    {


        public static string ObtieneJSONValoresDemo(this Plantilla plantilla)
        {
            string json = "{  %C% }";
            string baseProp = "'%N%': '%V%',";
            
            StringBuilder sb = new StringBuilder();

            List<string> palabras = "Tal vez amar es aprendera caminar por este mundo.Aprender a quedarnos quietoscomo el tilo y la encina de la fábula.Aprender a mirar.Tu mirada es sembradora.Plantó un árbol Yo habloporque tú meces los follajes".Split(' ').ToList();
            string demoBytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
            string boolValue = DateTime.Now.Second%2==0? "true" : "false";
            string numDouble = DateTime.Now.Second.ToString() + ".25" ;
            Random random = new Random();
            DateTime date = DateTime.Now;
            DateTime dateD = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime dateT = new DateTime(2000, 1, 1, date.Hour, date.Minute, date.Second);

            int len = random.Next(palabras.Count-1);
            if (len == 0) len = 10;
            string text = "";
            for (int i = 0; i< len; i++)
            {
                text += palabras[i] + " ";
            }

            foreach (var item in plantilla.Propiedades)
            {
                switch (item.TipoDatoId)
                {

                    case TipoDato.tBinaryData:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", demoBytes ));
                        break;

                    case TipoDato.tBoolean:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", boolValue));
                        break;

                    case TipoDato.tDouble:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", numDouble));
                        break;

                    case TipoDato.tInt32:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", DateTime.Now.Second.ToString()));
                        break;

                    case TipoDato.tInt64:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", DateTime.Now.Millisecond.ToString()));
                        break;

                    case TipoDato.tList:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", palabras[len]));
                        break;

                    case TipoDato.tString:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", text));
                        break;

                    case TipoDato.tTime:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", dateT.ToString("o")));
                        break;

                    case TipoDato.tDate:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", dateD.ToString("o")));
                        break;

                    case TipoDato.tDateTime:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", date.ToString("o")));
                        break;

                }
            }

            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                return json.Replace("%C%", campos).Replace('\'', '\"');
            }

            return "";
        }

        public static ValoresPlantilla ObtieneValoresDemo(this Plantilla plantilla)
        {
            ValoresPlantilla valores= new ValoresPlantilla() {
                Id = System.Guid.NewGuid().ToString(),
                OrigenId = System.Guid.NewGuid().ToString(),
                PlantillaId = plantilla.Id,
                TipoOrigenId = "demo"
            };


            string json = "{  %C% }";
            string baseProp = "'%N%': '%V%',";

            StringBuilder sb = new StringBuilder();

            List<string> palabras = "Tal vez amar es aprendera caminar por este mundo.Aprender a quedarnos quietoscomo el tilo y la encina de la fábula.Aprender a mirar.Tu mirada es sembradora.Plantó un árbol Yo habloporque tú meces los follajes".Split(' ').ToList();
            string demoBytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
            string boolValue = DateTime.Now.Second % 2 == 0 ? "true" : "false";
            string numDouble = DateTime.Now.Second.ToString() + ".25";
            Random random = new Random();
            DateTime date = DateTime.Now;
            DateTime dateD = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime dateT = new DateTime(2000, 1, 1, date.Hour, date.Minute, date.Second);

            int len = random.Next(palabras.Count - 1);
            if (len == 0) len = 10;
            string text = "";
            for (int i = 0; i < len; i++)
            {
                text += palabras[i] + " ";
            }

            foreach (var item in plantilla.Propiedades)
            {
                switch (item.TipoDatoId)
                {

                    case TipoDato.tBinaryData:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = demoBytes });
                        break;

                    case TipoDato.tBoolean:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = boolValue });
                        break;

                    case TipoDato.tDouble:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = numDouble });
                        break;

                    case TipoDato.tInt32:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = DateTime.Now.Second.ToString() });
                        break;

                    case TipoDato.tInt64:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = DateTime.Now.Millisecond.ToString() });
                        break;

                    case TipoDato.tList:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = palabras[len] });
                        break;

                    case TipoDato.tString:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = text});
                        break;

                    case TipoDato.tTime:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = dateT.ToString("o") });
                        break;

                    case TipoDato.tDate:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = dateD.ToString("o") });

                        break;

                    case TipoDato.tDateTime:
                        valores.Valores.Add(new ValorPropiedad() { PropiedadId = item.Id, Valor = date.ToString("o") });

                        break;

                }
            }

            return valores;
        }


        public static string ObtieneJSONValores(this ValoresPlantilla valores, Plantilla plantilla ) {
            string json = "{  %C% }";
            string baseProp = "'%N%': '%V%',";

            StringBuilder sb = new StringBuilder();

            foreach (var item in plantilla.Propiedades)
            {
                var valor = valores.Valores.Where(x => x.PropiedadId == item.Id).SingleOrDefault();
                if(valor!=null) sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", valor.Valor));
            }

            if (sb.Length > 0)
            {
                sb.Append($"'OrigenId': '{valores.OrigenId}',");
                sb.Append($"'TipoOrigenId': '{valores.TipoOrigenId}',");
            }
            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                return json.Replace("%C%", campos).Replace('\'', '\"');
            }

            return "";
        }


        public static string ObtieneJSONPlantilla(this Plantilla plantilla)
        {
            string json = "{ 'mappings': { 'properties': { %C% } }}";
            string baseProp = @"'%N%': {'type': '%T%'},";

            StringBuilder sb = new StringBuilder();
            
            

            foreach(var item in plantilla.Propiedades)
            {
                switch (item.TipoDatoId) {
                    
                    case TipoDato.tBinaryData:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "binary"));
                        break;

                    case TipoDato.tBoolean:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "boolean"));
                        break;

                    case TipoDato.tDouble:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "double"));
                        break;

                    case TipoDato.tInt32:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "integer"));
                        break;

                    case TipoDato.tInt64:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "long"));
                        break;

                        
                    case TipoDato.tList:
                    case TipoDato.tString:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "text"));
                        break;

                    case TipoDato.tTime:
                    case TipoDato.tDate:
                    case TipoDato.tDateTime:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "date"));

                        break;

                }
            }

            if (sb.Length > 0)
            {
                sb.Append(baseProp.Replace("%N%", "OrigenId").Replace("%T%", "text"));
                sb.Append(baseProp.Replace("%N%", "TipoOrigenId").Replace("%T%", "text"));
            }

            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                return json.Replace("%C%", campos).Replace('\'','\"');
            }

            return "";
        }



    }
}
