using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;

namespace PIKA.Servicio.Seguridad.Data
{
  public  class InicializarDatos
    {
        public static void Inicializar(DbContextSeguridad dbContext, string contentPath, bool datosDemo)
        {
            DatosIniciales(dbContext);
            InicializarGeneros(dbContext);
            
        }
        private static void DatosIniciales(
           DbContextSeguridad dbContext)
        {

            UsuarioDominio u = new UsuarioDominio()
            {
                ApplicationUserId = "admin",
                DominioId = "principal",
                EsAdmin = true,
                UnidadOrganizacionalId = "principal"
            };

            if (dbContext.UsuariosDominio.Find(u.ApplicationUserId) == null)
            {
                dbContext.UsuariosDominio.Add(u);
                dbContext.SaveChanges();
            }

        }

            private static void InicializarGeneros(
           DbContextSeguridad dbContext)
        {
 
                Genero g = new Genero();
                List<Genero> lista = g.Seed();


                foreach (var o  in lista)
                {

                    var instancia = dbContext.Generos.Find(o.Id);
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

    }
}
