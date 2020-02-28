using PIKA.Infraestructura.Comun.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIKA.UI.Web
{
    public class ServicioInyectable
    {
        public string RutaEnsamblado { get; set; }
        public string NombreServicio { get; set; }
        public string NombreImplementacion { get; set; }

    }


    public class LozalizadorEnsamblados
    {

        public static List<ServicioInyectable> ObtieneServiciosInyectables() {
            List<ServicioInyectable> l = new List<ServicioInyectable>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });
            
            foreach(var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(IServicioInyectable).IsAssignableFrom(t))
                            .ToArray();
                    foreach(var t in Tipos)
                    {
                        foreach(var i in t.GetInterfaces())
                        {
                            if(("i" +  t.Name.ToLower())==i.Name.ToLower())
                            {
                                ServicioInyectable s = new ServicioInyectable()
                                {
                                    NombreImplementacion = t.FullName,
                                    NombreServicio = i.FullName,
                                    RutaEnsamblado = item
                                };
                                    
                                l.Add(s);
                            }
                            
                        }

                    }


                }
                catch (Exception)
                {

                }
            }

            return l;
        }

        public static string ObtieneRutaBin()
        {
            return new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
        }

        public static List<string> LocalizaConTipo(string path, Type type)
        {
            var assemblies = Directory.GetFiles(path, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });
            List<string> lista = new List<string>();
            foreach (string s in Localiza(assemblies, type))
            {
                lista.Add(s);
            }

            return lista;
        }

        private static IReadOnlyCollection<string> Localiza(string[] assemblyPaths, Type type)
        {
            var assemblyPluginInfos = new List<string>();

            foreach (var assemblyPath in assemblyPaths)
            {

                try
                {
                    var assembly = Assembly.LoadFile(assemblyPath);
                    if (AbalizaTipo(assembly, type).Any())
                    {
                        if (assemblyPluginInfos.IndexOf(assembly.Location) < 0)
                            assemblyPluginInfos.Add(assembly.Location);
                    }

                }
                catch (Exception)
                {
                 
                }
            }

            return assemblyPluginInfos;
        }

        public static IReadOnlyCollection<Type> AbalizaTipo(Assembly assembly, Type type)
        {
            return assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            type.IsAssignableFrom(t))
                            .ToArray();
        }


    }
}

 