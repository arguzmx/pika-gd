using Microsoft.CodeAnalysis.CSharp.Syntax;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public static class ElasticJSONExtender
    {

        public static string ObtieneJSONValores(this ValoresPlantilla valores, Plantilla plantilla ) {
            string json = "{ 'mappings': { 'properties': { %C% } }}";

            return json;
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


            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                return json.Replace("%C%", campos).Replace('\'','\"');
            }

            return "";
        }



    }
}
