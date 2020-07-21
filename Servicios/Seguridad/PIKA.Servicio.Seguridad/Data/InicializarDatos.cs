using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad.Data
{
  public  class InicializarDatos
    {
        public static void Inicializar(DbContextSeguridad dbContext, string contentPath, bool datosDemo)
        {
            InicializarGeneros(dbContext);
            
        }


        private static void InicializarGeneros(
           DbContextSeguridad dbContext)
        {

            try
            {
                Genero g = new Genero();
                List<Genero> lista = g.Seed();


                foreach (var o  in lista)
                {

                    var instancia = dbContext.ModuloAplicacion.Find(o.Id);
                    if (instancia == null)
                    {
                        Genero p = new Genero()
                        {
                            Id = o.Id,
                            Nombre = o.Nombre
                        };

                        dbContext.Generos.Add(p);
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

    }
}
