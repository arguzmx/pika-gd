using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Seguridad.Data
{
  public  class InicializarDatos
    {
        public static void Inicializar(DbContextSeguridad dbContext, string contentPath)
        {
            InicializarAplicacion(dbContext, contentPath);
            InicializarModuloAplicacion(dbContext, contentPath);
            InicializarTipoAdministradorModulo(dbContext, contentPath);
            InicializarTraduccionAplicacionModulo(dbContext, contentPath);
        }
        private static void InicializarAplicacion(
     DbContextSeguridad dbContext, string contentPath)
        {
            try
            {
                List<Aplicacion> aplicacion = new List<Aplicacion>();

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
                            aplicacion.Add(new Aplicacion()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {aplicacion.Count} elementos");

                foreach (Aplicacion aplicaciones in aplicacion)
                {

                    Aplicacion instancia = dbContext.Aplicacion.Find(aplicaciones.Id);
                    if (instancia == null)
                    {
                        Aplicacion p = new Aplicacion()
                        {
                            Id = aplicaciones.Id,
                            Nombre = aplicaciones.Nombre
                        };

                        dbContext.Aplicacion.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = aplicaciones.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarModuloAplicacion(
           DbContextSeguridad dbContext, string contentPath)
        {

            try
            {
                List<ModuloAplicacion> moduloaplicacion = new List<ModuloAplicacion>();

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
                            moduloaplicacion.Add(new ModuloAplicacion()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {moduloaplicacion.Count} elementos");

                foreach (ModuloAplicacion modulosaplicacion in moduloaplicacion)
                {

                    ModuloAplicacion instancia = dbContext.ModuloAplicacion.Find(modulosaplicacion.Id);
                    if (instancia == null)
                    {
                        ModuloAplicacion p = new ModuloAplicacion()
                        {
                            Id = modulosaplicacion.Id,
                            Nombre = modulosaplicacion.Nombre
                        };

                        dbContext.ModuloAplicacion.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = modulosaplicacion.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarTipoAdministradorModulo(
           DbContextSeguridad dbContext, string contentPath)
        {

            try
            {
                List<TipoAdministradorModulo> TipoAdministradorModulo = new List<TipoAdministradorModulo>();

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
                            TipoAdministradorModulo.Add(new TipoAdministradorModulo()
                            {
                                Id = partes[4],
                                ModuloId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {TipoAdministradorModulo.Count} elementos");

                foreach (TipoAdministradorModulo tipoadministradormodulo in TipoAdministradorModulo)
                {

                    TipoAdministradorModulo instancia = dbContext.TipoAdministradorModulo.Find(tipoadministradormodulo.Id);
                    if (instancia == null)
                    {
                        TipoAdministradorModulo p = new TipoAdministradorModulo()
                        {
                            Id = tipoadministradormodulo.Id,
                            ModuloId = tipoadministradormodulo.ModuloId
                        };

                        dbContext.TipoAdministradorModulo.Add(p);
                    }
                    else
                    {
                        instancia.ModuloId = tipoadministradormodulo.ModuloId;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private static void InicializarTraduccionAplicacionModulo(
           DbContextSeguridad dbContext, string contentPath)
        {

            try
            {
                List<TraduccionAplicacionModulo> TraduccionAplicacionModulo = new List<TraduccionAplicacionModulo>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "TraduccionAplicacionModulo.txt");

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
                            TraduccionAplicacionModulo.Add(new TraduccionAplicacionModulo()
                            {
                                Id = partes[4],
                                ModuloId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {TraduccionAplicacionModulo.Count} elementos");

                foreach (TraduccionAplicacionModulo traduccionaplicacionmodulo in TraduccionAplicacionModulo)
                {

                    TraduccionAplicacionModulo instancia = dbContext.TraduccionAplicacionModulo.Find(traduccionaplicacionmodulo.Id);
                    if (instancia == null)
                    {
                        TraduccionAplicacionModulo p = new TraduccionAplicacionModulo()
                        {
                            Id = traduccionaplicacionmodulo.Id,
                            ModuloId = traduccionaplicacionmodulo.ModuloId
                        };

                        dbContext.TraduccionAplicacionModulo.Add(p);
                    }
                    else
                    {
                        instancia.ModuloId = traduccionaplicacionmodulo.ModuloId;
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
