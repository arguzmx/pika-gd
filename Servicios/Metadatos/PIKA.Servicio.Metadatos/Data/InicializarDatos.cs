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

            InicializarTipoDato(dbContext, contentPath);
            GeneraTiposAlmacenDefault(dbContext);

            GeneraDatosDemo(dbContext);
        }

        private static void GeneraTiposAlmacenDefault(DbContextMetadatos dbContext) {
            
            TipoAlmacenMetadatos t = new TipoAlmacenMetadatos();
            List<TipoAlmacenMetadatos> tipos = t.Seed();
            

            foreach (TipoAlmacenMetadatos tipo in tipos)
            {

                TipoAlmacenMetadatos instancia = dbContext.TipoAlmacenMetadatos.Find(tipo.Id);
                if (instancia == null)
                {
                    TipoAlmacenMetadatos p = new TipoAlmacenMetadatos()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.TipoAlmacenMetadatos.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }

        private static void GeneraDatosDemo(DbContextMetadatos dbContext)
        {


            AlmacenDatos al = new AlmacenDatos()
            {
                Contrasena = null,
                Direccion = "localhost",
                Id = "almdemo",
                Nombre = "Default Elasticsearch",
                Protocolo = "http://",
                Puerto = "9200",
                TipoAlmacenMetadatosId = TipoAlmacenMetadatos.tElasticSearch,
                Usuario = null
            };

            dbContext.AlmacenesDatos.Add(al);

            Plantilla plantilla = new Plantilla()
            {
                Id = "demo",
                Eliminada = false,
                Nombre = "Plantilla demo",
                OrigenId = "dominio",
                TipoOrigenId = "dominio",
                AlmacenDatosId = al.Id
            };

            Plantilla temp = dbContext.Plantilla.Where(x => x.Id == plantilla.Id).Take(1).SingleOrDefault();
            if (temp!=null)
            {
                temp.Eliminada = plantilla.Eliminada;
                temp.Nombre = plantilla.Nombre;
                temp.OrigenId = plantilla.OrigenId;
                temp.TipoOrigenId = plantilla.TipoOrigenId;
                temp.AlmacenDatosId = plantilla.AlmacenDatosId;
                dbContext.Plantilla.Update(temp);

            } else
            {
                dbContext.Plantilla.Add(plantilla);
            }

            PropiedadPlantilla prop;
            ValidadorNumero vnum;
            ValidadorTexto vtext;

           
            

        dbContext.SaveChanges();


        }

        private static void InicializarTipoDato(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                TipoDato t = new TipoDato();
                List<TipoDato> tipodato = t.Seed();


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


        private static void InicializarPlantilla
            (DbContextMetadatos dbContext, string contentPath)
        {

            try
            {
                List<Plantilla> plantilla = new List<Plantilla>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "Plantilla.txt");


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

 
   
        private static void InicializarAtributoTabla(DbContextMetadatos dbContext, string contentPath)
        {
            try
            {
                List<AtributoTabla> atributotabla = new List<AtributoTabla>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "AtributoTabla.txt");


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
