using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido.Data
{
    public class InicializarDatos
    {
        public static void Inicializar(DbContextContenido dbContext, string contentPath)
        {
            InicializarTipoGestorES(dbContext, contentPath);
        }

        private static void InicializarTipoGestorES(
           DbContextContenido dbContext, string contentPath)
        {

            try
            {
                TipoGestorES entidad = new TipoGestorES();
                List<TipoGestorES> TipoGestorES = entidad.Seed();


                foreach (TipoGestorES item in TipoGestorES)
                {

                    TipoGestorES instancia = dbContext.TipoGestorES.Find(item.Id);
                    if (instancia == null)
                    {
                        TipoGestorES p = new TipoGestorES()
                        {
                            Id = item.Id,
                            Nombre = item.Nombre
                        };

                        dbContext.TipoGestorES.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = item.Nombre;
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
