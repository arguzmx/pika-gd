using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    }
}
