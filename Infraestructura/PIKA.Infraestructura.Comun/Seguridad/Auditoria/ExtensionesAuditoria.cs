using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PIKA.Infraestructura.Comun.Seguridad.Auditoria
{
    public static class ExtensionesAuditoria
    {
        public static JsonSerializerSettings FlatSettings(int depth =1)
        {
            return new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, MaxDepth = depth, 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        }

        /// <summary>
        /// Serializa removiendo las propoedades nulas 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="depth"></param>
        /// <param name="NulificarColecciones"></param>
        /// <returns></returns>
        public static string Flat(this object o, int depth = 1, bool NulificarColecciones = true) 
        {
            o = Nulificar(o, NulificarColecciones);
            return JsonConvert.SerializeObject(o, FlatSettings(depth));
        }

        /// <summary>
        /// ASigna como null todos las propiedades que no están en el espacio de nombres System
        /// </summary>
        /// <param name="o"></param>
        /// <param name="NulificarColecciones"></param>
        /// <returns></returns>
        public static object Nulificar(object o, bool NulificarColecciones = true)
        {
            PropertyInfo[] info = o.GetType().GetProperties();
            foreach(var p in info)
            {
                if (!p.PropertyType.Namespace.Equals("System"))
                {
                    if (p.PropertyType.Namespace.StartsWith("System.Collections"))
                    {
                        if (NulificarColecciones)
                        {
                            p.SetValue(o, null);
                        }

                    } else
                    {
                        p.SetValue(o, null);
                    }
                }
            }

            return o;
        }

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
            var patch = new JsonDiffPatch(new Options() {  TextDiff = TextDiffMode.Efficient }).Diff(left, right);
            var formatter = new JsonDeltaFormatter();
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
