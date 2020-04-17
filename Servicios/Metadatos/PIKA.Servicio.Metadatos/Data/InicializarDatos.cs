using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos.Data
{
    public class InicializarDatos
    {
        public static void Inicializar(DbContextMetadatos dbContext, string contentPath)
        {
            InicializarPlantilla(dbContext, contentPath);
            InicializarPropiedadPlantilla(dbContext, contentPath);
            InicializarTipoDato(dbContext, contentPath);
            InicializarTipoDatoPropiedadPlantilla(dbContext, contentPath);
            InicializarAtributoMetadato(dbContext, contentPath);
            InicializarAtributoTabla(dbContext, contentPath);
            InicializarValidadorNumero(dbContext, contentPath);
            InicializarAsociacionPlantilla(dbContext, contentPath);
            InicializarTipoAlmacenMetadatos(dbContext, contentPath);
        }
        private static void InicializarPlantilla
            (DbContextMetadatos dbContext, string contentPath)
        {

            try
            {
                List<Plantilla> plantilla = new List<Plantilla>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "Plantilla.txt");

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
                            plantilla.Add(new Plantilla()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {plantilla.Count} elementos");

                foreach (Plantilla plantillas in plantilla)
                {

                    Plantilla instancia = dbContext.Plantilla.Find(plantillas.Id);
                    if (instancia == null)
                    {
                        Plantilla p = new Plantilla()
                        {
                            Id = plantillas.Id,
                            Nombre = plantillas.Nombre
                        };

                        dbContext.Plantilla.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = plantillas.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarPropiedadPlantilla(DbContextMetadatos dbContext, string contentPath) 
        {
            try
            {
                List<PropiedadPlantilla> propiedadplantilla = new List<PropiedadPlantilla>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "PropiedadPlantilla.txt");

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
                            propiedadplantilla.Add(new PropiedadPlantilla()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {propiedadplantilla.Count} elementos");

                foreach (PropiedadPlantilla propiedadesplantilla in propiedadplantilla)
                {

                    PropiedadPlantilla instancia = dbContext.PropiedadPlantilla.Find(propiedadesplantilla.Id);
                    if (instancia == null)
                    {
                        PropiedadPlantilla p = new PropiedadPlantilla()
                        {
                            Id = propiedadesplantilla.Id,
                            Nombre = propiedadesplantilla.Nombre
                        };

                        dbContext.PropiedadPlantilla.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = propiedadesplantilla.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarTipoDato(DbContextMetadatos dbContext, string contentPath)
        {

            try
            {
                List<TipoDato> tipodato = new List<TipoDato>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "TipoDato.txt");

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
                            tipodato.Add(new TipoDato()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {tipodato.Count} elementos");

                foreach (TipoDato TipoDatos in tipodato)
                {

                    TipoDato instancia = dbContext.TipoDato.Find(TipoDatos.Id);
                    if (instancia == null)
                    {
                        TipoDato p = new TipoDato()
                        {
                            Id = TipoDatos.Id,
                            Nombre = TipoDatos.Nombre
                        };

                        dbContext.TipoDato.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = TipoDatos.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarTipoDatoPropiedadPlantilla(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<TipoDatoPropiedadPlantilla> tipodatopropiedadplantilla = new List<TipoDatoPropiedadPlantilla>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "TipoDatoPropiedadPlantilla.txt");

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
                            tipodatopropiedadplantilla.Add(new TipoDatoPropiedadPlantilla()
                            {
                                PropiedadPlantillaId = partes[4],
                                TipoDatoId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {tipodatopropiedadplantilla.Count} elementos");

                foreach (TipoDatoPropiedadPlantilla tipodatospropiedadplantilla in tipodatopropiedadplantilla)
                {

                    TipoDatoPropiedadPlantilla instancia = dbContext.TipoDatoPropiedadPlantilla.Find(tipodatospropiedadplantilla.PropiedadPlantillaId);
                    if (instancia == null)
                    {
                        TipoDatoPropiedadPlantilla p = new TipoDatoPropiedadPlantilla()
                        {
                            PropiedadPlantillaId = tipodatospropiedadplantilla.PropiedadPlantillaId,
                            TipoDatoId = tipodatospropiedadplantilla.TipoDatoId
                        };

                        dbContext.TipoDatoPropiedadPlantilla.Add(p);
                    }
                    else
                    {
                        instancia.PropiedadPlantillaId = tipodatospropiedadplantilla.PropiedadPlantillaId;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarAtributoMetadato(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<AtributoMetadato> atributometadato = new List<AtributoMetadato>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "AtributoMetadato.txt");

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
                            atributometadato.Add(new AtributoMetadato()
                            {
                                Id = partes[4],
                                Valor = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {atributometadato.Count} elementos");

                foreach (AtributoMetadato atributosmetadato in atributometadato)
                {

                    AtributoMetadato instancia = dbContext.AtributoMetadato.Find(atributosmetadato.Id);
                    if (instancia == null)
                    {
                        AtributoMetadato p = new AtributoMetadato()
                        {
                            Id = atributosmetadato.Id,
                            Valor = atributosmetadato.Valor
                        };

                        dbContext.AtributoMetadato.Add(p);
                    }
                    else
                    {
                        instancia.Id = atributosmetadato.Id;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private static void InicializarAtributoTabla(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<AtributoTabla> atributotabla = new List<AtributoTabla>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "AtributoTabla.txt");

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
                            atributotabla.Add(new AtributoTabla()
                            {
                                Id = partes[4],
                                PropiedadId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {atributotabla.Count} elementos");

                foreach (AtributoTabla atributostabla in atributotabla)
                {

                    AtributoTabla instancia = dbContext.AtributoTabla.Find(atributostabla.Id);
                    if (instancia == null)
                    {
                        AtributoTabla p = new AtributoTabla()
                        {
                            Id = atributostabla.Id,
                            PropiedadId = atributostabla.PropiedadId
                        };

                        dbContext.AtributoTabla.Add(p);
                    }
                    else
                    {
                        instancia.Id = atributostabla.Id;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private static void InicializarValidadorNumero(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<ValidadorNumero> validadornumero = new List<ValidadorNumero>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "ValidadorNumero.txt");

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
                            validadornumero.Add(new ValidadorNumero()
                            {
                                Id = partes[4],
                                PropiedadId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {validadornumero.Count} elementos");

                foreach (ValidadorNumero validadornumeros in validadornumero)
                {

                    ValidadorNumero instancia = dbContext.ValidadorNumero.Find(validadornumeros.Id);
                    if (instancia == null)
                    {
                        ValidadorNumero p = new ValidadorNumero()
                        {
                            Id = validadornumeros.Id,
                            PropiedadId = validadornumeros.PropiedadId
                        };

                        dbContext.ValidadorNumero.Add(p);
                    }
                    else
                    {
                        instancia.Id = validadornumeros.Id;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private static void InicializarValidadorTexto(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<ValidadorTexto> validadortexto = new List<ValidadorTexto>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "ValidadorTexto.txt");

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
                            validadortexto.Add(new ValidadorTexto()
                            {
                                Id = partes[4],
                                PropiedadId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {validadortexto.Count} elementos");

                foreach (ValidadorTexto validadortextos in validadortexto)
                {

                    ValidadorTexto instancia = dbContext.ValidadorTexto.Find(validadortextos.Id);
                    if (instancia == null)
                    {
                        ValidadorTexto p = new ValidadorTexto()
                        {
                            Id = validadortextos.Id,
                            PropiedadId = validadortextos.PropiedadId
                        };

                        dbContext.ValidadorTexto.Add(p);
                    }
                    else
                    {
                        instancia.Id = validadortextos.Id;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private static void InicializarAsociacionPlantilla(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<AsociacionPlantilla> asociacionplantilla = new List<AsociacionPlantilla>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "AsociacionPlantilla.txt");

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
                            asociacionplantilla.Add(new AsociacionPlantilla()
                            {
                                Id = partes[4],
                                PlantillaId = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {asociacionplantilla.Count} elementos");

                foreach (AsociacionPlantilla asociacionplantillas in asociacionplantilla)
                {

                    AsociacionPlantilla instancia = dbContext.AsociacionPlantilla.Find(asociacionplantillas.Id);
                    if (instancia == null)
                    {
                        AsociacionPlantilla p = new AsociacionPlantilla()
                        {
                            Id = asociacionplantillas.Id,
                            PlantillaId = asociacionplantillas.PlantillaId
                        };

                        dbContext.AsociacionPlantilla.Add(p);
                    }
                    else
                    {
                        instancia.Id = asociacionplantillas.Id;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private static void InicializarTipoAlmacenMetadatos(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<TipoAlmacenMetadatos> tipoalmacenmetadatos = new List<TipoAlmacenMetadatos>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "TipoAlmacenMetadatos.txt");

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
                            tipoalmacenmetadatos.Add(new TipoAlmacenMetadatos()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {tipoalmacenmetadatos.Count} elementos");

                foreach (TipoAlmacenMetadatos tiposalmacenmetadatos in tipoalmacenmetadatos)
                {

                    TipoAlmacenMetadatos instancia = dbContext.TipoAlmacenMetadatos.Find(tiposalmacenmetadatos.Id);
                    if (instancia == null)
                    {
                        TipoAlmacenMetadatos p = new TipoAlmacenMetadatos()
                        {
                            Id = tiposalmacenmetadatos.Id,
                            Nombre = tiposalmacenmetadatos.Nombre
                        };

                        dbContext.TipoAlmacenMetadatos.Add(p);
                    }
                    else
                    {
                        instancia.Id = tiposalmacenmetadatos.Id;
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
