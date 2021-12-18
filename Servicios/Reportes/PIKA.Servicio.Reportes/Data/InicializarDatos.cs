using Microsoft.Extensions.Logging;
using PIKA.Modelo.Reportes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PIKA.Servicio.Reportes.Data
{
   public class InicializarDatos
    {

        private ILogger localLogger;
        private void InsertaRreporte(DbContextReportes dbContext, ReporteEntidad r, FileInfo fi)
        {
            LogDebug($"Verificando reporte {r.Id}{r.ExtensionSalida}");
            if (!dbContext.ReporteEntidades.Where(x => x.Id == r.Id && x.OrigenId == r.OrigenId
                                   && x.TipoOrigenId == r.TipoOrigenId).Any())
            {
                LogDebug($"Añadiendo {r.Id}{r.ExtensionSalida}");
                r.Plantilla = Convert.ToBase64String(File.ReadAllBytes(fi.FullName));
                dbContext.ReporteEntidades.Add(r);
                dbContext.SaveChanges();
            } else
            {
                if (dbContext.ReporteEntidades.Where(x => x.Id == r.Id && x.OrigenId == r.OrigenId
                              && x.TipoOrigenId == r.TipoOrigenId && x.Bloqueado == false).Any())
                {
                    LogDebug($"Actualizando {r.Id}{r.ExtensionSalida}");
                    r.Plantilla = Convert.ToBase64String(File.ReadAllBytes(fi.FullName));
                    dbContext.ReporteEntidades.Update(r);
                    dbContext.SaveChanges();
                } else
                {
                    LogDebug($"Reporte existente bloqueado {r.Id}{r.ExtensionSalida}");
                }
            }
        }

        private void LogDebug(string msg)
        {
            if (localLogger != null)
            {
                localLogger.LogDebug(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        private string ObtieneRutaBin()
        {
            return new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
        }


        public  void Inicializar(DbContextReportes dbContext, string contentPath, ILogger logger = null)
        {
            localLogger = logger;
            LogDebug($"Procesando reportes");
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });
            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                             typeof(IRepoReportes).IsAssignableFrom(t))
                            .ToArray();

                    foreach (var t in Tipos)
                    {
                        LogDebug($"Procesando reportes de {t.FullName}");
                        var instancia = assembly.CreateInstance(t.FullName);
                        List<ReporteEntidad> reportes = ((IRepoReportes)instancia).ObtieneReportes();
                        reportes.ForEach(r =>
                        {
                            string NombrePlantilla = $"{r.Id}{r.ExtensionSalida}";
                            string RutaReporte = Path.Combine(contentPath, "Contenido", "Reportes", NombrePlantilla);
                            if (File.Exists(RutaReporte))
                            {
                                FileInfo fi = new FileInfo(RutaReporte);
                                InsertaRreporte(dbContext, r, fi);
                            } else
                            {
                                LogDebug($"El archivo para el reporte {r.Id} no existe");
                            }
                            
                        });


                    }

                }
                catch (Exception)
                {

                }
            }

        }

    }
}
