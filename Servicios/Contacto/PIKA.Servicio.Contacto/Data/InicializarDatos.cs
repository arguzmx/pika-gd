using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public static class InicializarDatos
    {

        public static void Inicializar(DbContextContacto dbContext, string contentPath, bool datosDemo)
        {
            InicializarPaises(dbContext, contentPath);
            GeneraTiposDeMediosDefault(dbContext);
            GeneraTiposFuenteDefault(dbContext);

        }
        private static void GeneraTiposDeMediosDefault(DbContextContacto dbContext)
        {

            TipoMedio t = new TipoMedio();
            List<TipoMedio> tipos = t.Seed();


            foreach (TipoMedio tipo in tipos)
            {

                TipoMedio instancia = dbContext.TiposMedio.Find(tipo.Id);
                if (instancia == null)
                {
                    TipoMedio p = new TipoMedio()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.TiposMedio.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }
        private static void GeneraTiposFuenteDefault(DbContextContacto dbContext)
        {

            TipoFuenteContacto t = new TipoFuenteContacto();
            List<TipoFuenteContacto> tipos = t.Seed();


            foreach (TipoFuenteContacto tipo in tipos)
            {

                TipoFuenteContacto instancia = dbContext.TiposFuentesContacto.Find(tipo.Id);
                if (instancia == null)
                {
                    TipoFuenteContacto p = new TipoFuenteContacto()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.TiposFuentesContacto.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }



        private static void InicializarPaises(DbContextContacto dbContext, string contentPath)
        {

            try
            {
                List<Pais> paises = new List<Pais>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "paises.txt");


                if (File.Exists(path))
                {


                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {

                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            paises.Add(new Pais()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }


                foreach (Pais pais in paises)
                {

                    Pais instancia = dbContext.Paises.Find(pais.Id);
                    if (instancia == null)
                    {
                        Pais p = new Pais()
                        {
                            Id = pais.Id,
                            Nombre = pais.Nombre
                        };

                        dbContext.Paises.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = pais.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }


            try
            {
                List<Estado> estados = new List<Estado>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "estados.txt");


                if (File.Exists(path))
                {


                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {

                        if (index > 0)
                        {

                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            if (partes.Count >= 4)
                            {
                                estados.Add(new Estado()
                                {
                                    Id = partes[2],
                                    Nombre = partes[0],
                                    PaisId = partes[3]
                                });
                            }

                        }
                        index++;
                    }
                }


                foreach (Estado estado in estados)
                {

                    Estado instancia = dbContext.Estados.Find(estado.Id);
                    if (instancia == null)
                    {
                        Estado p = new Estado()
                        {
                            Id = estado.Id,
                            Nombre = estado.Nombre,
                            PaisId = estado.PaisId
                        };

                        dbContext.Estados.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = estado.Nombre;
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
