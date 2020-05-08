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
            InicializarEstadosCuadroClasificacion(dbContext, contentPath);
            InicializarTiposArchivo(dbContext, contentPath);
            InicializarFasesCicloVital(dbContext, contentPath);
        }

        private static void InicializarEstadosCuadroClasificacion(DBContextGestionDocumental dbContext, string contentPath)
        {
            try
            {
                List<EstadoCuadroClasificacion> estadosCuadro = new List<EstadoCuadroClasificacion>();
                string path = Path.Combine(contentPath, "Data", "Inicializar", "estadosCuadro.txt");

                if (File.Exists(path))
                {
                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {
                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            estadosCuadro.Add(new EstadoCuadroClasificacion()
                            {
                                Id = partes[4],
                                Nombre = partes[0]
                            });

                        }
                        index++;
                    }
                }

                foreach (EstadoCuadroClasificacion edo in estadosCuadro)
                {

                    EstadoCuadroClasificacion instancia = dbContext.EstadosCuadroClasificacion.Find(edo.Id);
                    if (instancia == null)
                    {
                        EstadoCuadroClasificacion p = new EstadoCuadroClasificacion()
                        {
                            Id = edo.Id,
                            Nombre = edo.Nombre
                        };

                        dbContext.EstadosCuadroClasificacion.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = edo.Nombre;
                    }
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private static void InicializarTiposArchivo(DBContextGestionDocumental dbContext, string contentPath)
        {
            //try
            //{
            //    List<TipoArchivo> tiposArchivo = new List<TipoArchivo>();
            //    string path = Path.Combine(contentPath, "Data", "Inicializar", "tiposArchivo.txt");

            //    if (File.Exists(path))
            //    {
            //        int index = 0;
            //        List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
            //        foreach (string linea in lineas)
            //        {
            //            if (index > 0)
            //            {
            //                List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
            //                tiposArchivo.Add(new TipoArchivo()
            //                {
            //                    Id = partes[4],
            //                    Nombre = partes[0]
            //                });

            //            }
            //            index++;
            //        }
            //    }

            //    foreach (TipoArchivo tipo in tiposArchivo)
            //    {
            //        TipoArchivo instancia = dbContext.TiposArchivo.Find(tipo.Id);
            //        if (instancia == null)
            //        {
            //            TipoArchivo p = new TipoArchivo()
            //            {
            //                Id = tipo.Id,
            //                Nombre = tipo.Nombre
            //            };

            //            dbContext.TiposArchivo.Add(p);
            //        }
            //        else
            //        {
            //            instancia.Nombre = tipo.Nombre;
            //        }
            //    }
            //    dbContext.SaveChanges();
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

        }

        private static void InicializarFasesCicloVital(DBContextGestionDocumental dbContext, string contentPath)
        {
            //try
            //{
            //    List<FaseCicloVital> fases = new List<FaseCicloVital>();
            //    string path = Path.Combine(contentPath, "Data", "Inicializar", "fasesCicloVital.txt");

            //    if (File.Exists(path))
            //    {
            //        int index = 0;
            //        List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
            //        foreach (string linea in lineas)
            //        {
            //            if (index > 0)
            //            {
            //                List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
            //                fases.Add(new FaseCicloVital()
            //                {
            //                    Id = partes[4],
            //                    Nombre = partes[0]
            //                });

            //            }
            //            index++;
            //        }
            //    }

            //    foreach (FaseCicloVital fase in fases)
            //    {
            //        FaseCicloVital instancia = dbContext.FasesCicloVital.Find(fase.Id);
            //        if (instancia == null)
            //        {
            //            FaseCicloVital p = new FaseCicloVital()
            //            {
            //                Id = fase.Id,
            //                Nombre = fase.Nombre
            //            };

            //            dbContext.FasesCicloVital.Add(p);
            //        }
            //        else
            //        {
            //            instancia.Nombre = fase.Nombre;
            //        }
            //    }
            //    dbContext.SaveChanges();
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

        }
    }
}
