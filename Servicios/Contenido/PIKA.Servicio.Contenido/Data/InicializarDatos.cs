using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido.Data
{
  public  class InicializarDatos
    {
        public static void Inicializar(DbContextContenido dbContext, string contentPath)
        {
            InicializarElemento(dbContext, contentPath);
            InicializarParte(dbContext, contentPath);
            InicializarTipoGestorES(dbContext, contentPath);
            InicializarVersion(dbContext, contentPath);
            InicializarVolumen(dbContext, contentPath);
        }
        private static void InicializarElemento(
     DbContextContenido dbContext, string contentPath)
        {
            try
            {
                List<Elemento> Elemento = new List<Elemento>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "Elemento.txt");

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
                            Elemento.Add(new Elemento()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {Elemento.Count} elementos");

                foreach (Elemento Elementos in Elemento)
                {

                    Elemento instancia = dbContext.Elemento.Find(Elementos.Id);
                    if (instancia == null)
                    {
                        Elemento p = new Elemento()
                        {
                            Id = Elementos.Id,
                            Nombre = Elementos.Nombre
                        };

                        dbContext.Elemento.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = Elementos.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarParte(
           DbContextContenido dbContext, string contentPath)
        {

            try
            {
                List<Parte> Parte = new List<Parte>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "Parte.txt");

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
                            Parte.Add(new Parte()
                            {
                                ElementoId = partes[4],
                                VersionId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {Parte.Count} elementos");

                foreach (Parte Partes in Parte)
                {

                    Parte instancia = dbContext.Parte.Find(Partes.ElementoId);
                    if (instancia == null)
                    {
                        Parte p = new Parte()
                        {
                            ElementoId = Partes.ElementoId,
                            VersionId = Partes.VersionId
                        };

                        dbContext.Parte.Add(p);
                    }
                    else
                    {
                        instancia.ElementoId = Partes.ElementoId;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarTipoGestorES(
           DbContextContenido dbContext, string contentPath)
        {

            try
            {
                List<TipoGestorES> TipoGestorES = new List<TipoGestorES>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "TipoGestorES.txt");

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
                            TipoGestorES.Add(new TipoGestorES()
                            {
                                Id = partes[4],
                                 Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {TipoGestorES.Count} elementos");

                foreach (TipoGestorES TiposGestorES in TipoGestorES)
                {

                    TipoGestorES instancia = dbContext.TipoGestorES.Find(TiposGestorES.Id);
                    if (instancia == null)
                    {
                        TipoGestorES p = new TipoGestorES()
                        {
                            Id = TiposGestorES.Id,
                             Nombre = TiposGestorES.Nombre
                        };

                        dbContext.TipoGestorES.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = TiposGestorES.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private static void InicializarVersion(
              DbContextContenido dbContext, string contentPath)
        {

            try
            {
                List<Modelo.Contenido.Version> versions = new List<Modelo.Contenido.Version>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "Version.txt");

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
                            versions.Add(new Modelo.Contenido.Version()
                            {
                                Id = partes[4],
                                ElementoId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {versions.Count} elementos");

                foreach (Modelo.Contenido.Version versiones in versions)
                {

                    Modelo.Contenido.Version instancia = dbContext.Version.Find(versiones.Id);
                    if (instancia == null)
                    {
                        Modelo.Contenido.Version p = new Modelo.Contenido.Version()
                        {
                            Id = versiones.Id,
                            ElementoId = versiones.ElementoId
                        };

                        dbContext.Version.Add(p);
                    }
                    else
                    {
                        instancia.ElementoId = versiones.ElementoId;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private static void InicializarVolumen(
                 DbContextContenido dbContext, string contentPath)
        {

            try
            {
                List<Volumen> Volumenes = new List<Volumen>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "Volumen.txt");

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
                            Volumenes.Add(new Volumen()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {Volumenes.Count} elementos");

                foreach (Volumen volumen in Volumenes)
                {

                    Volumen instancia = dbContext.Volumen.Find(volumen.Id);
                    if (instancia == null)
                    {
                       Volumen p = new Volumen()
                        {
                            Id = volumen.Id,
                            Nombre = volumen.Nombre
                        };

                        dbContext.Volumen.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = volumen.Nombre;
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
