using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace PIKA.Infraestructura.Comun.Seguridad.Auditoria
{
    public static class ExtensionesAuditoria
    {
        /// <summary>
        /// Regresa la diferencia de la serialización JSON para los objetos
        /// </summary>
        /// <param name="original"></param>
        /// <param name="modificado"></param>
        /// <returns></returns>
        public static string JsonDiff(this string original, string modificado)
        {
            var left = JObject.Parse(original);
            var right = JObject.Parse(modificado);
            var patch = new JsonDiffPatch().Diff(left, right);
            //var formatter = new JsonDeltaFormatter();
            //var operations = formatter.Format(patch);

            //if (operations.Count> 0)
            //{
            //    return JsonConvert.SerializeObject(operations);
            //}
            return (patch == null ? null : JsonConvert.SerializeObject(patch));
        }

        public static List<EventosComunesAuditables> TodosEventosComunes()
        {
            return new List<EventosComunesAuditables>() {
             EventosComunesAuditables.Crear,
             EventosComunesAuditables.Actualizar,
             EventosComunesAuditables.Eliminar,
             EventosComunesAuditables.Leer,
             EventosComunesAuditables.Purgar
            };
        }

        public static List<TipoEventoAuditoria> ObtieneTiposEvento<T>(string  AppId, string ModuloId, string Nombre, bool TodosComunes = true, List<EventosComunesAuditables> EventosIndividuales = null )
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();



            if(TodosComunes)
            {
                
            } else
            {
                if (EventosIndividuales != null)
                {

                }
            }



            return l;
        }

        public static TipoEventoAuditoria CreaTipoEvento(string AppId, string ModuloId, int Tipo, string Descripcion)
        {
            return  new TipoEventoAuditoria() { AppId = AppId, Desripcion = Descripcion, ModuloId = ModuloId, PlantillaEvento = null, TipoEvento = Tipo };
        }

    }
}
