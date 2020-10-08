using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infrastructure.EventBus.Abstractions;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PIKA.GD.API
{

    public class ServicioInyectable
    {
        public string RutaEnsamblado { get; set; }
        public string NombreServicio { get; set; }
        public string NombreImplementacion { get; set; }
    }


    public class EnsambladosEvento
    {
        public string RutaEnsambladoHandler { get; set; }
        public string RutaEnsambladoEvento { get; set; }
        public string NombreHandler { get; set; }
        public string NombreEvento { get; set; }
    }


    public static class LocalizadorEnsamblados
    {

        public static Type ObtieneTipoMetadata(Type tipo)
        {

            foreach (var c in tipo.GetConstructors())
            {
                foreach (var p in c.GetParameters())
                {
                    if (p.ParameterType.FullName.Contains("IProveedorMetadatos", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var a in p.ParameterType.GenericTypeArguments)
                        {
                            return a;
                        }
                    }
                }
            }

            return null;
        }

        public static List<Type> ObtieneContextosInicializables()
        {
            List<Type> l = new List<Type>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll",
                new EnumerationOptions() { RecurseSubdirectories = true });

            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(IRepositorioInicializable).IsAssignableFrom(t))
                            .ToArray();

                    l.AddRange(Tipos);


                }
                catch (Exception)
                {

                }
            }

            return l;
        }


        public static List<RutaTipo> ObtieneControladoresACL()
        {
            List<RutaTipo> rutas = new List<RutaTipo>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });
            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            t.IsSubclassOf(typeof(ControllerBase)) &&
                            !t.IsAbstract)
                            .ToArray();

                    foreach (var t in Tipos)
                    {

                        

                        object[] TypeAttrs = t.GetCustomAttributes(true);
                        string TemplateRuteo = "";
                        foreach (object attr in TypeAttrs)
                        {
                            if (attr is RouteAttribute)
                            {
                                TemplateRuteo = ((RouteAttribute)attr).Template.Replace("[controller]", t.Name.Replace("Controller","") );
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(TemplateRuteo))
                        {
                            foreach (var c in t.GetConstructors())
                            {
                                foreach (var p in c.GetParameters())
                                {
                                    if (p.ParameterType.FullName.Contains("IProveedorMetadatos", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        foreach (var a in p.ParameterType.GenericTypeArguments)
                                        {
                                            rutas.Add(new RutaTipo()
                                            {
                                                Ruta = TemplateRuteo,
                                                Tipo = a.Name,
                                                Version = ""
                                            });
                                        }
                                    }
                                }
                            }
                        }

                   }

                }
                catch (Exception)
                {

                }
            }

            return rutas;
        }

        public static List<TipoAdministradorModulo> ObtieneTiposAdministrados()
        {
            List<TipoAdministradorModulo> l = new List<TipoAdministradorModulo>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });

            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(IInformacionAplicacion).IsAssignableFrom(t))
                            .ToArray();

                    foreach (var t in Tipos)
                    {
                        TipoAdministradorModulo s = new TipoAdministradorModulo();
                        var instancia = assembly.CreateInstance(t.FullName);
                        List<TipoAdministradorModulo> tmp = ((IInformacionAplicacion)instancia).TiposAdministrados();
                        l.AddRange(tmp);

                    }


                }
                catch (Exception)
                {

                }
            }

            return l;
        }


        public static List<ServicioInyectable> ObtieneServiciosBusEventos()
        {
            List<ServicioInyectable> l = new List<ServicioInyectable>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });

            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(IEventBusService).IsAssignableFrom(t))
                            .ToArray();
                    foreach (var t in Tipos)
                    {

                        foreach (var i in t.GetInterfaces())
                        {
                            if ((("I" + t.Name).ToUpperInvariant()) == i.Name.ToUpperInvariant())
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



        public static List<ServicioInyectable> ObtieneServiciosInyectables()
        {
            List<ServicioInyectable> l = new List<ServicioInyectable>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });

            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(IServicioInyectable).IsAssignableFrom(t))
                            .ToArray();
                    foreach (var t in Tipos)
                    {

                        foreach (var i in t.GetInterfaces())
                        {
                            if ((("I" + t.Name).ToUpperInvariant()) == i.Name.ToUpperInvariant())
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


        public static List<EnsambladosEvento> ObtieneManejadoresEventosBus()
        {
            List<EnsambladosEvento> l = new List<EnsambladosEvento>();
            string Ruta = ObtieneRutaBin();

            var assemblies = Directory.GetFiles(Ruta, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });

            foreach (var item in assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFile(item);
                    var Tipos = assembly.GetTypes()
                            .Where(t =>
                            !t.IsAbstract &&
                            typeof(IIntegrationEventHandler).IsAssignableFrom(t))
                            .ToArray();

                    foreach (var t in Tipos)
                    {
                        foreach (var i in t.GetInterfaces())
                        {
                            if (i.Name.IndexOf("IIntegrationEventHandler") >= 0)
                            {
                                if (i.IsGenericType)
                                {

                                    foreach (var a in i.GenericTypeArguments)
                                    {

                                        EnsambladosEvento s = new EnsambladosEvento()
                                        {
                                            NombreEvento = a.FullName,
                                            NombreHandler = t.FullName,
                                            RutaEnsambladoEvento = item,
                                            RutaEnsambladoHandler = item
                                        };

                                        l.Add(s);
                                    }

                                }
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
