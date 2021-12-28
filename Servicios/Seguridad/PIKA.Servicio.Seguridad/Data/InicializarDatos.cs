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

            if (!dbContext.UsuariosDominio.Any(x => x.ApplicationUserId == u.ApplicationUserId 
                && x.DominioId == u.DominioId
                && x.UnidadOrganizacionalId == u.UnidadOrganizacionalId))
            {
                dbContext.UsuariosDominio.Add(u);
                dbContext.SaveChanges();
            }

            if(!dbContext.PropiedadesUsuario.Any(x=>x.UsuarioId == "admin"))
            {
                PropiedadesUsuario p = new PropiedadesUsuario()
                {
                    Eliminada = false,
                    email = "admin",
                    email_verified = true,
                    family_name = "De sistema",
                    given_name = ".",
                    Inactiva = false,
                    name = "Administrador",
                    middle_name = ".",
                    nickname = "admin",
                    username = "admin",
                    UsuarioId = "admin"
                };
                dbContext.PropiedadesUsuario.Add(p);
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
