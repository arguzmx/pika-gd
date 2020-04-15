using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace PIKA.Servicio.Organizacion
{
    public class InicializarDatos
    {
        public static void Inicializar(DbContextOrganizacion dbContext, string contentPath) {
            Console.WriteLine("Inicializando Listado de paise");
            InicializarPaises(dbContext, contentPath);
        }

        private static void InicializarPaises(DbContextOrganizacion dbContext, string contentPath) {

            try
            {
                List<Pais> paises = new List<Pais>();

                string path = Path.Combine(contentPath, "Data", "Inicializar", "paises.txt");

                Console.WriteLine($"Buscando archivo en {path}");

                if (File.Exists(path)){

                    
                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas) {
                        if (index > 0)
                        {
                        
                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            paises.Add(new Pais()
                            {
                                Id = partes[4],
                                Valor = partes[0]
                            });

                        }
                        index++;
                    }
                }

                Console.WriteLine($"Actualizando {paises.Count} elementos");

                foreach (Pais pais in paises) {

                    Pais instancia = dbContext.Paises.Find(pais.Id);
                    if (instancia == null)
                    {
                        Pais p = new Pais()
                        {
                            Id = pais.Id,
                            Valor = pais.Valor
                        };

                        dbContext.Paises.Add(p);
                    }
                    else {
                        instancia.Valor = pais.Valor;
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
