using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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

        public static List<TipoEventoAuditoria> EventoComunes<T>(this string PLuralEntidad, string AppId, string ModuloId)
        {

            Type t = typeof(T);

            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>
            {
                CreaTipoEvento(AppId, ModuloId, EventosComunesAuditables.Crear.GetHashCode(), $"C-{PLuralEntidad}", t.Name),
                CreaTipoEvento(AppId, ModuloId, EventosComunesAuditables.Actualizar.GetHashCode(), $"U-{PLuralEntidad}", t.Name),
                CreaTipoEvento(AppId, ModuloId, EventosComunesAuditables.Eliminar.GetHashCode(), $"D-{PLuralEntidad}", t.Name),
                CreaTipoEvento(AppId, ModuloId, EventosComunesAuditables.Leer.GetHashCode(), $"R-{PLuralEntidad}", t.Name),
            };
            //l.Add(CreaTipoEvento(AppId, ModuloId, EventosComunesAuditables.Purgar.GetHashCode(), $"Purgar {PLuralEntidad}"));
            return l;
        }

        public static TipoEventoAuditoria CreaTipoEvento(string AppId, string ModuloId, int Tipo, string Descripcion, string TipoEntidad)
        {
            return  new TipoEventoAuditoria() { AppId = AppId, Descripcion = Descripcion, ModuloId = ModuloId, PlantillaEvento = null, TipoEvento = Tipo, TipoEntidad = TipoEntidad };
        }

    }
}
