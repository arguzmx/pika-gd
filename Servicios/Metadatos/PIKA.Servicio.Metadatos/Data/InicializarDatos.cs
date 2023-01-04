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
            GeneraAlmcenDatosDefault(dbContext);
            // GeneraDatosDemo(dbContext);
        }

        private static void GeneraAlmcenDatosDefault(DbContextMetadatos dbContext)
        {
            AlmacenDatos am = new AlmacenDatos()
            {
                Contrasena = "",
                Direccion = "localhost",
                Id = "default",
                Nombre = "DefaultElasticsearch",
                Protocolo = "",
                Puerto = "",
                TipoAlmacenMetadatosId = "esearch",
                Usuario = ""
            };

            AlmacenDatos instancia = dbContext.AlmacenesDatos.Find(am.Id);
            if (instancia == null)
            {
                dbContext.AlmacenesDatos.Add(am);
            }
            dbContext.SaveChanges();
        }

        private static void GeneraTiposAlmacenDefault(DbContextMetadatos dbContext) {

            TipoAlmacenMetadatos te = new TipoAlmacenMetadatos();
            List<TipoAlmacenMetadatos> tipos = te.Seed();


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


    }
}
