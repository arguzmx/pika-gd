﻿using PIKA.Modelo.GestorDocumental;
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
            InicializarFasesCicloVital(dbContext, contentPath);
            InicializarTiposArchivo(dbContext, contentPath);
            InicializarEstadosTransferencia(dbContext, contentPath);
            InicializarTiposAmpliacion(dbContext, contentPath);
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
                                Id = partes[0],
                                Nombre = partes[1]
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

        private static void InicializarFasesCicloVital(DBContextGestionDocumental dbContext, string contentPath)
        {
            try
            {
                List<FaseCicloVital> fases = new List<FaseCicloVital>();
                string path = Path.Combine(contentPath, "Data", "Inicializar", "fasesCicloVital.txt");

                if (File.Exists(path))
                {
                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {
                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            fases.Add(new FaseCicloVital()
                            {
                                Id = partes[0],
                                Nombre = partes[1]
                            });

                        }
                        index++;
                    }
                }

                foreach (FaseCicloVital fase in fases)
                {
                    FaseCicloVital instancia = dbContext.FasesCicloVital.Find(fase.Id);
                    if (instancia == null)
                    {
                        FaseCicloVital p = new FaseCicloVital()
                        {
                            Id = fase.Id,
                            Nombre = fase.Nombre
                        };

                        dbContext.FasesCicloVital.Add(p);
                    }
                    else
                    {
                        instancia.Nombre = fase.Nombre;
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
            try
            {
                List<TipoArchivo> tiposArchivo = new List<TipoArchivo>();
                string path = Path.Combine(contentPath, "Data", "Inicializar", "tiposArchivo.txt");

                if (File.Exists(path))
                {
                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {
                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            tiposArchivo.Add(new TipoArchivo()
                            {
                                Id = partes[0],
                                Nombre = partes[1],
                                FaseCicloVitalId = partes[2]
                            });

                        }
                        index++;
                    }
                }

                foreach (TipoArchivo tipo in tiposArchivo)
                {
                    TipoArchivo instancia = dbContext.TiposArchivo.Find(tipo.Id);
                    if (instancia == null)
                    {
                        TipoArchivo p = new TipoArchivo()
                        {
                            Id = tipo.Id,
                            Nombre = tipo.Nombre,
                            FaseCicloVitalId = tipo.FaseCicloVitalId,
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
            catch (Exception ex)
            {

                throw;
            }

        }

        private static void InicializarEstadosTransferencia(DBContextGestionDocumental dbContext, string contentPath)
        {
            try
            {
                List<EstadoTransferencia> estados = new List<EstadoTransferencia>();
                string path = Path.Combine(contentPath, "Data", "Inicializar", "estadosTransferencia.txt");

                if (File.Exists(path))
                {
                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {
                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            estados.Add(new EstadoTransferencia()
                            {
                                Id = partes[0],
                                Nombre = partes[1],
                            });

                        }
                        index++;
                    }
                }

                foreach (EstadoTransferencia tipo in estados)
                {
                    EstadoTransferencia instancia = dbContext.EstadosTransferencia.Find(tipo.Id);
                    if (instancia == null)
                    {
                        EstadoTransferencia p = new EstadoTransferencia()
                        {
                            Id = tipo.Id,
                            Nombre = tipo.Nombre,
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
            catch (Exception ex)
            {

                throw;
            }

        }

        private static void InicializarTiposAmpliacion(DBContextGestionDocumental dbContext, string contentPath)
        {
            try
            {
                List<TipoAmpliacion> estados = new List<TipoAmpliacion>();
                string path = Path.Combine(contentPath, "Data", "Inicializar", "tiposAmpliacion.txt");

                if (File.Exists(path))
                {
                    int index = 0;
                    List<string> lineas = File.ReadAllText(path).Split('\n').ToList();
                    foreach (string linea in lineas)
                    {
                        if (index > 0)
                        {
                            List<string> partes = linea.TrimStart().TrimEnd().Split('\t').ToList();
                            estados.Add(new TipoAmpliacion()
                            {
                                Id = partes[0],
                                Nombre = partes[1],
                            });

                        }
                        index++;
                    }
                }

                foreach (TipoAmpliacion tipo in estados)
                {
                    TipoAmpliacion instancia = dbContext.TiposAmpliaciones.Find(tipo.Id);
                    if (instancia == null)
                    {
                        TipoAmpliacion p = new TipoAmpliacion()
                        {
                            Id = tipo.Id,
                            Nombre = tipo.Nombre,
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
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
