using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin.Data
{
  public  class InicializarDatos
    {
        public static void Inicializar(DbContextAplicacionPlugin dbContext, string contentPath)
        {
            InicializarPlugin(dbContext, contentPath);
            InicializarPluginInstalado(dbContext, contentPath);
            InicializarVesionPlugin(dbContext, contentPath);
        }
        private static void InicializarPlugin(
     DbContextAplicacionPlugin dbContext, string contentPath)
        {
            try
            {
                List<Plugin> plugin = new List<Plugin>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "Aplicacion.txt");

                Console.WriteLine($"Buscando archivo en {path}");

                if (File.Exists(path))
                {


                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {

                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            plugin.Add(new Plugin()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {plugin.Count} elementos");

                foreach (Plugin plugins in plugin)
                {

                    Plugin instancia = dbContext.Plugin.Find(plugins.Id);
                    if (instancia == null)
                    {
                        Plugin p = new Plugin()
                        {
                            Id = plugins.Id,
                            Nombre = plugins.Nombre
                        };

                        dbContext.Plugin.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = plugins.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarPluginInstalado(
           DbContextAplicacionPlugin dbContext, string contentPath)
        {

            try
            {
                List<PluginInstalado> plugininstalado = new List<PluginInstalado>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "ModuloAplicacion.txt");

                Console.WriteLine($"Buscando archivo en {path}");

                if (File.Exists(path))
                {


                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {

                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            plugininstalado.Add(new PluginInstalado()
                            {
                                PLuginId = partes[4],
                                VersionPLuginId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {plugininstalado.Count} elementos");

                foreach (PluginInstalado plugininstalados in plugininstalado)
                {

                    PluginInstalado instancia = dbContext.PluginInstalado.Find(plugininstalados.PLuginId);
                    if (instancia == null)
                    {
                        PluginInstalado p = new PluginInstalado()
                        {
                            PLuginId = plugininstalados.PLuginId,
                            VersionPLuginId = plugininstalados.VersionPLuginId
                        };

                        dbContext.PluginInstalado.Add(p);
                    }
                    else
                    {
                        instancia.VersionPLuginId = plugininstalados.VersionPLuginId;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarVesionPlugin(
           DbContextAplicacionPlugin dbContext, string contentPath)
        {

            try
            {
                List<VersionPlugin> VersionPlugin = new List<VersionPlugin>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "TipoAdministradorModulo.txt");

                Console.WriteLine($"Buscando archivo en {path}");

                if (File.Exists(path))
                {


                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {

                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            VersionPlugin.Add(new VersionPlugin()
                            {
                                Id = partes[4],
                                Version = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {VersionPlugin.Count} elementos");

                foreach (VersionPlugin VersionPlugins in VersionPlugin)
                {

                    VersionPlugin instancia = dbContext.VersionPlugin.Find(VersionPlugins.Id);
                    if (instancia == null)
                    {
                        VersionPlugin p = new VersionPlugin()
                        {
                            Id = VersionPlugins.Id,
                            Version = VersionPlugins.Version
                        };

                        dbContext.VersionPlugin.Add(p);
                    }
                    else
                    {
                        instancia.Version = VersionPlugins.Version;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    
    }
}
