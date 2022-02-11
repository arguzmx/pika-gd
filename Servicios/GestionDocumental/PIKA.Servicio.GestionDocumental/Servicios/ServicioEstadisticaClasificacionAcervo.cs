using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public partial class ServicioEstadisticaClasificacionAcervo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioEstadisticaClasificacionAcervo
    {
        private IRepositorioAsync<EstadisticaClasificacionAcervo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private readonly ConfiguracionServidor configuracion;
        public ServicioEstadisticaClasificacionAcervo
            (
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            IOptions<ConfiguracionServidor> Confi,
            ILogger<ServicioLog> l)
            : base(proveedorOpciones, l)
        {
            this.configuracion = Confi.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<EstadisticaClasificacionAcervo>(new QueryComposer<EstadisticaClasificacionAcervo>());
        }


        public async Task ActualizarConteos(string ArchivoId)
        {
            string sqls = $"delete from {DBContextGestionDocumental.TablaEstadisticaClasificacionAcervo} where ArchivoId = '{ArchivoId}';";
            await this.UDT.Context.Database.ExecuteSqlRawAsync(sqls);

            sqls = $@"insert into {DBContextGestionDocumental.TablaEstadisticaClasificacionAcervo} (ArchivoId, UnidadAdministrativaArchivoId, CuadroClasificacionId, EntradaClasificacionId, ConteoActivos, ConteoActivosEliminados, FechaMinApertura, FechaMaxCierre)
                      select ArchivoId, UnidadAdministrativaArchivoId, CuadroClasificacionId, EntradaClasificacionId, count(*) as ConteoActivos, 0 as ConteoActivosEliminados, min(FechaApertura) as FechaMinApertura, max(FechaCierre) as FechaMaxCierre 
                      from {DBContextGestionDocumental.TablaActivos} where ArchivoId = '{ArchivoId}' and Eliminada=false group by ArchivoId, UnidadAdministrativaArchivoId, CuadroClasificacionId, EntradaClasificacionId;";
            await this.UDT.Context.Database.ExecuteSqlRawAsync(sqls);


            sqls = $@"select ArchivoId, UnidadAdministrativaArchivoId, CuadroClasificacionId, EntradaClasificacionId, 0 as ConteoActivos, count(*) as ConteoActivosEliminados, null as FechaMinApertura, null as FechaMaxCierre 
                      from gd$activo where ArchivoId = '{ArchivoId}' and Eliminada=true group by ArchivoId, UnidadAdministrativaArchivoId, CuadroClasificacionId, EntradaClasificacionId;";
            List<EstadisticaClasificacionAcervo> eliminados = await this.UDT.Context.EstadisticaClasificacionAcervo.FromSqlRaw(sqls).ToListAsync();
            List<EstadisticaClasificacionAcervo> vigentes = await this.UDT.Context.EstadisticaClasificacionAcervo.Where(x=>x.ArchivoId == ArchivoId).ToListAsync();

            eliminados.ForEach(e =>
            {
                if(vigentes.Any(x=>x.CuadroClasificacionId == e.CuadroClasificacionId && x.EntradaClasificacionId == e.EntradaClasificacionId 
                && x.ArchivoId == e.ArchivoId && x.UnidadAdministrativaArchivoId == e.UnidadAdministrativaArchivoId))
                {
                    sqls = $"update {DBContextGestionDocumental.TablaEstadisticaClasificacionAcervo} set ConteoActivosEliminados = {e.ConteoActivosEliminados} where ArchivoId = '{e.ArchivoId}' and UnidadAdministrativaArchivoId = '{e.UnidadAdministrativaArchivoId}' and CuadroClasificacionId = '{e.CuadroClasificacionId}' and EntradaClasificacionId = '{e.EntradaClasificacionId}';";
                    this.UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                } else
                {
                    this.UDT.Context.EstadisticaClasificacionAcervo.Add(e);
                    this.UDT.Context.SaveChanges();
                }
            });


        }

        public async Task AdicionaEstadistica(EstadisticaClasificacionAcervo estadistica)
        {
            try
            {
                this.UDT.Context.Database.BeginTransaction();

                var r = await this.UDT.Context.EstadisticaClasificacionAcervo.Where(x => x.ArchivoId == estadistica.ArchivoId
                && x.UnidadAdministrativaArchivoId == estadistica.UnidadAdministrativaArchivoId
                && x.CuadroClasificacionId == estadistica.CuadroClasificacionId
                && x.EntradaClasificacionId == estadistica.EntradaClasificacionId).FirstOrDefaultAsync();

                if (r == null)
                {
                    estadistica.ConteoActivos = 1;
                    estadistica.ConteoActivosEliminados = 0;
                    this.UDT.Context.EstadisticaClasificacionAcervo.Add(estadistica);
                    await this.UDT.Context.SaveChangesAsync();

                }
                else
                {
                    r.ConteoActivos++;
                    if (estadistica.FechaMinApertura != null)
                    {
                        if (r.FechaMinApertura != null)
                        {
                            if (r.FechaMinApertura > estadistica.FechaMinApertura)
                            {
                                r.FechaMinApertura = estadistica.FechaMinApertura;
                            }
                        }
                        else
                        {
                            r.FechaMinApertura = estadistica.FechaMinApertura;
                        }
                    }

                    if (estadistica.FechaMaxCierre != null)
                    {
                        if (r.FechaMaxCierre != null)
                        {
                            if (r.FechaMaxCierre < estadistica.FechaMaxCierre)
                            {
                                r.FechaMaxCierre = estadistica.FechaMaxCierre;
                            }
                        }
                        else
                        {
                            r.FechaMaxCierre = estadistica.FechaMaxCierre;
                        }
                    }
                    this.UDT.Context.EstadisticaClasificacionAcervo.Update(r);
                    await this.UDT.Context.SaveChangesAsync();
                }

                this.UDT.Context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                this.UDT.Context.Database.RollbackTransaction();
                Console.WriteLine(ex.ToString());
                throw;
            }

        }

        public async Task EliminaEstadistica(EstadisticaClasificacionAcervo estadistica, bool CambioDocumental = false)
        {
            try
            {
                this.UDT.Context.Database.BeginTransaction();

                var r = await this.UDT.Context.EstadisticaClasificacionAcervo.Where(x => x.ArchivoId == estadistica.ArchivoId
                && x.UnidadAdministrativaArchivoId == estadistica.UnidadAdministrativaArchivoId
                && x.CuadroClasificacionId == estadistica.CuadroClasificacionId
                && x.EntradaClasificacionId == estadistica.EntradaClasificacionId).FirstOrDefaultAsync();

                if (r == null)
                {
                    estadistica.ConteoActivos = 0;
                    estadistica.ConteoActivosEliminados = 1;
                    this.UDT.Context.EstadisticaClasificacionAcervo.Add(estadistica);
                    await this.UDT.Context.SaveChangesAsync();
                }
                else
                {
                    r.ConteoActivos --;

                    // Si es un cambio documental no es necesario marcarlo como eliminado
                    if (!CambioDocumental)
                    {
                        r.ConteoActivosEliminados++;
                    }
                    
                    
                    if (r.ConteoActivos < 0)
                    {
                        r.ConteoActivos = 0;
                    }

                    if (estadistica.FechaMinApertura.HasValue)
                    {
                        Activo min = this.UDT.Context.Activos.Where(x => x.Eliminada == false && x.FechaApertura < estadistica.FechaMinApertura).OrderBy(x => x.FechaApertura).Take(1).SingleOrDefault();
                        if (min == null)
                        {
                            min = this.UDT.Context.Activos.Where(x => x.Eliminada == false && x.FechaApertura > estadistica.FechaMinApertura).OrderBy(x => x.FechaApertura).Take(1).SingleOrDefault();
                            if (min != null)
                            {
                                estadistica.FechaMinApertura = min.FechaApertura;
                            }
                        }
                    }

                    if (estadistica.FechaMaxCierre.HasValue)
                    {
                        Activo max = this.UDT.Context.Activos.Where(x => x.Eliminada == false && x.FechaCierre > estadistica.FechaMaxCierre).OrderByDescending(x => x.FechaApertura).Take(1).SingleOrDefault();
                        if (max == null)
                        {
                            max = this.UDT.Context.Activos.Where(x => x.Eliminada == false && x.FechaCierre < estadistica.FechaMaxCierre).OrderByDescending(x => x.FechaApertura).Take(1).SingleOrDefault();
                            if (max != null)
                            {
                                estadistica.FechaMaxCierre = max.FechaCierre;
                            }
                        }
                    }

                    this.UDT.Context.EstadisticaClasificacionAcervo.Update(r);
                    await this.UDT.Context.SaveChangesAsync();
                }

                this.UDT.Context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                this.UDT.Context.Database.RollbackTransaction();
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task ActualizaEstadistica(EstadisticaClasificacionAcervo estadistica, int adicionar, int eliminar)
        {
            try
            {
                this.UDT.Context.Database.BeginTransaction();

                var r = await this.UDT.Context.EstadisticaClasificacionAcervo.Where(x => x.ArchivoId == estadistica.ArchivoId
                && x.UnidadAdministrativaArchivoId == estadistica.UnidadAdministrativaArchivoId
                && x.CuadroClasificacionId == estadistica.CuadroClasificacionId
                && x.EntradaClasificacionId == estadistica.EntradaClasificacionId).FirstOrDefaultAsync();

                if (r == null)
                {
                    estadistica.ConteoActivos = 1;
                    estadistica.ConteoActivosEliminados = 0;
                    this.UDT.Context.EstadisticaClasificacionAcervo.Add(estadistica);
                    await this.UDT.Context.SaveChangesAsync();
                }
                else
                {
                    r.ConteoActivos += adicionar;
                    r.ConteoActivosEliminados += eliminar;

                    if (r.ConteoActivos < 0)
                    {
                        r.ConteoActivos = 0;
                    }

                    if (estadistica.FechaMinApertura != null)
                    {
                        if (r.FechaMinApertura != null)
                        {
                            if (r.FechaMinApertura > estadistica.FechaMinApertura)
                            {
                                r.FechaMinApertura = estadistica.FechaMinApertura;
                            }
                        }
                        else
                        {
                            r.FechaMinApertura = estadistica.FechaMinApertura;
                        }
                    }

                    if (estadistica.FechaMaxCierre != null)
                    {
                        if (r.FechaMaxCierre != null)
                        {
                            if (r.FechaMaxCierre < estadistica.FechaMaxCierre)
                            {
                                r.FechaMaxCierre = estadistica.FechaMaxCierre;
                            }
                        }
                        else
                        {
                            r.FechaMaxCierre = estadistica.FechaMaxCierre;
                        }
                    }

                    this.UDT.Context.EstadisticaClasificacionAcervo.Update(r);
                    await this.UDT.Context.SaveChangesAsync();
                }

                this.UDT.Context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                this.UDT.Context.Database.RollbackTransaction();
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
