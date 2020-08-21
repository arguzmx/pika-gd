using PIKA.Modelo.GestorDocumental;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data
{
    public class InicializarDatos
    {
        public static void Inicializar(DBContextGestionDocumental dbContext, string contentPath)
        {
            GeneraEstadoCuadroClasificacionDefault(dbContext);
            GeneraEstadoTransferenciaDefault(dbContext);
            GeneraTiposAmpliacionDefault(dbContext);
            GeneraTipoValoracionDocumentalDefault(dbContext);
            GeneraTipoDisposicionDocumentalDefault(dbContext);
            GeneraTipoArchivoDefault(dbContext);
        }

        private static void GeneraTipoValoracionDocumentalDefault(DBContextGestionDocumental dbContext)
        {

            TipoValoracionDocumental t = new TipoValoracionDocumental();
            List<TipoValoracionDocumental> tipos = t.Seed();


            foreach (TipoValoracionDocumental tipo in tipos)
            {

                TipoValoracionDocumental instancia = dbContext.TipoValoracionDocumental.Find(tipo.Id);
                if (instancia == null)
                {
                    TipoValoracionDocumental p = new TipoValoracionDocumental()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.TipoValoracionDocumental.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }

        private static void GeneraTipoDisposicionDocumentalDefault(DBContextGestionDocumental dbContext)
        {

            TipoDisposicionDocumental t = new TipoDisposicionDocumental();
            List<TipoDisposicionDocumental> tipos = t.Seed();


            foreach (TipoDisposicionDocumental tipo in tipos)
            {

                TipoDisposicionDocumental instancia = dbContext.TipoDisposicionDocumental.Find(tipo.Id);
                if (instancia == null)
                {
                    TipoDisposicionDocumental p = new TipoDisposicionDocumental()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.TipoDisposicionDocumental.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }
        private static void GeneraEstadoCuadroClasificacionDefault(DBContextGestionDocumental dbContext)
        {

            EstadoCuadroClasificacion t = new EstadoCuadroClasificacion();
            List<EstadoCuadroClasificacion> tipos = t.Seed();


            foreach (EstadoCuadroClasificacion tipo in tipos)
            {

                EstadoCuadroClasificacion instancia = dbContext.EstadosCuadroClasificacion.Find(tipo.Id);
                if (instancia == null)
                {
                    EstadoCuadroClasificacion p = new EstadoCuadroClasificacion()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.EstadosCuadroClasificacion.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }
        
            private static void GeneraEstadoTransferenciaDefault(DBContextGestionDocumental dbContext)
        {

            EstadoTransferencia te = new EstadoTransferencia();
            List<EstadoTransferencia> tipos = te.Seed();


            foreach (EstadoTransferencia tipo in tipos)
            {

                EstadoTransferencia instancia = dbContext.EstadosTransferencia.Find(tipo.Id);
                if (instancia == null)
                {
                    EstadoTransferencia p = new EstadoTransferencia()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.EstadosTransferencia.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }
        private static void GeneraTiposAmpliacionDefault(DBContextGestionDocumental dbContext)
        {

            TipoAmpliacion t = new TipoAmpliacion();
            List<TipoAmpliacion> tipos = t.Seed();


            foreach (TipoAmpliacion tipo in tipos)
            {

                TipoAmpliacion instancia = dbContext.TiposAmpliaciones.Find(tipo.Id);
                if (instancia == null)
                {
                    TipoAmpliacion p = new TipoAmpliacion()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.TiposAmpliaciones.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }
        private static void GeneraTipoArchivoDefault(DBContextGestionDocumental dbContext)
        {

            TipoArchivo t = new TipoArchivo();
            List<TipoArchivo> tipos = t.Seed();


            foreach (TipoArchivo tipo in tipos)
            {
                TipoArchivo instancia = dbContext.TiposArchivo.Find(tipo.Id);
                if (instancia == null)
                {
                    TipoArchivo p = new TipoArchivo()
                    {
                        Id = tipo.Id,
                        Nombre = tipo.Nombre
                    };

                    dbContext.TiposArchivo.Add(p);
                }
                else
                {
                    instancia.Nombre = tipo.Nombre;
                }
            }
            dbContext.SaveChanges();

        }

    }
}
