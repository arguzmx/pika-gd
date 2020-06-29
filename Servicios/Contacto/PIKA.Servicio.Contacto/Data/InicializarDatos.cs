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
            Console.WriteLine("Inicializando Listado de paises y estados");
            InicializarPaises(dbContext, contentPath);
            InicalizaTiposDeMedios(dbContext, contentPath);
            InicalizaTiposFuente(dbContext, contentPath);

        }

        private static void InicalizaTiposFuente(DbContextContacto dbContext, string contentPath)
        {
            try
            {
                TipoFuenteContacto g = new TipoFuenteContacto();
                List<TipoFuenteContacto> lista = g.Seed();

                Console.WriteLine($"Añadiendo elementos Tipo Fuente Contacto");
                Console.WriteLine($"Actualizando {lista.Count} elementos");

                foreach (var o in lista)
                {

                    var instancia = dbContext.TiposFuentesContacto.Find(o.Id);
                    if (instancia == null)
                    {
                        TipoFuenteContacto p = new TipoFuenteContacto()
                        {
                            Id = o.Id,
                            Nombre = o.Nombre
                        };

                        dbContext.TiposFuentesContacto.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = o.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private static void InicalizaTiposDeMedios(DbContextContacto dbContext, string contentPath) {
            try
            {
                TipoMedio g = new TipoMedio();
                List<TipoMedio> lista = g.Seed();

                Console.WriteLine($"Añadiendo elementos Tipo Medio Contacto");
                Console.WriteLine($"Actualizando {lista.Count} elementos");

                foreach (var o in lista)
                {

                    var instancia = dbContext.TiposMedio.Find(o.Id);
                    if (instancia == null)
                    {
                        TipoMedio p = new TipoMedio()
                        {
                            Id = o.Id,
                            Nombre = o.Nombre
                        };

                        dbContext.TiposMedio.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = o.Nombre;
                    }
                }
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InicializarPaises(DbContextContacto dbContext, string contentPath)
        {

            try
            {
                List<Pais> paises = new List<Pais>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "paises.txt");

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
                            paises.Add(new Pais()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {paises.Count} elementos");

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

                Console.WriteLine($"Actualizando estados {estados.Count} elementos");

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
