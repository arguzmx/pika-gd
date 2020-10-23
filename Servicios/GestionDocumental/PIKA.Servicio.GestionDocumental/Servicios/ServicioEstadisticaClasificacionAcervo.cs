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


        public async Task<bool> ActualizaConteoEstadistica(string CuadroClasificacionId, string EntradaClasificacionId,
            string ArchivoId, int cantidadActivos, DateTime? fechaMinima, DateTime? fechaMaxima)
        {

            try
            {

                bool fechasCalculadas = false;
                EstadisticaClasificacionAcervo Estadistica = await this.repo.UnicoAsync(x =>
                x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase) &&
                x.EntradaClasificacionId.Equals(EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));

                if ((!fechaMaxima.HasValue) && (!fechaMaxima.HasValue))
                {
                    fechaMaxima = await UDT.Context.Activos.Where(x => x.Eliminada == false
                    && x.ArchivoId == ArchivoId
                    && x.EntradaClasificacionId == EntradaClasificacionId).MaxAsync(y => (DateTime?)y.FechaCierre) ?? null;

                    fechaMinima = await UDT.Context.Activos.Where(x => x.Eliminada == false
                    && x.ArchivoId == ArchivoId
                    && x.EntradaClasificacionId == EntradaClasificacionId).MaxAsync(y => (DateTime?)y.FechaApertura) ?? null;

                    if (fechaMinima == null) fechaMinima = null;
                    if (fechaMaxima == null) fechaMaxima = null;

                    fechasCalculadas = true;
                }

                // NO existe el registro
                if (Estadistica == null)
                {
                    Estadistica = new EstadisticaClasificacionAcervo()
                    {
                        ArchivoId = ArchivoId,
                        ConteoActivos = cantidadActivos < 0 ? 0 : cantidadActivos,
                        ConteoActivosEliminados = 0,
                        CuadroClasificacionId = CuadroClasificacionId,
                        EntradaClasificacionId = EntradaClasificacionId,
                        FechaMinApertura = fechaMinima,
                        FechaMaxCierre = fechaMaxima
                    };
                    await repo.CrearAsync(Estadistica);
                }
                else
                {
                    int cantidadEliminados = cantidadActivos * -1;
                    Estadistica.ConteoActivos += cantidadActivos;
                    Estadistica.ConteoActivosEliminados += cantidadEliminados;

                    if (Estadistica.ConteoActivos < 0) Estadistica.ConteoActivos = 0;
                    if (Estadistica.ConteoActivosEliminados < 0) Estadistica.ConteoActivosEliminados = 0;


                    if(fechasCalculadas)
                    {
                        Estadistica.FechaMinApertura = fechaMinima;
                        Estadistica.FechaMaxCierre = fechaMaxima;

                    } else
                    {
                        if (fechaMinima.HasValue)
                        {
                            if (Estadistica.FechaMinApertura.HasValue)
                            {
                                if (Estadistica.FechaMinApertura.Value.Ticks > fechaMinima.Value.Ticks)
                                {
                                    Estadistica.FechaMinApertura = fechaMinima;
                                }

                            }
                            else
                            {
                                Estadistica.FechaMinApertura = fechaMinima;
                            }
                        }


                        if (fechaMaxima.HasValue)
                        {
                            if (Estadistica.FechaMaxCierre.HasValue)
                            {
                                if (Estadistica.FechaMaxCierre.Value.Ticks < fechaMaxima.Value.Ticks)
                                {
                                    Estadistica.FechaMaxCierre = fechaMaxima;
                                }

                            }
                            else
                            {
                                Estadistica.FechaMaxCierre = fechaMaxima;
                            }
                        }

                    }

                    UDT.Context.Entry(Estadistica).State = EntityState.Modified;
                }

                UDT.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
        }


        public async Task ActualizarConteo(string ArchivoId)
        {
            await ActualizaConteoVigentes(ArchivoId);
            await ActualizaConteoEliminados(ArchivoId);
            await ActualizaFechasLimiteDesdeEstadistica(ArchivoId);
        }

        private async Task ActualizaFechasLimiteDesdeEstadistica(string ArchivoId)
        {

            foreach (var m in UDT.Context.EstadisticaClasificacionAcervo.Where(x => x.ArchivoId == ArchivoId).ToList())
            {
                DateTime? minima = await this.UDT.Context.Activos.Where(x => x.Eliminada == false
                && x.ArchivoId == ArchivoId
                && x.EntradaClasificacionId == m.EntradaClasificacionId)
                  .MinAsync(x => (DateTime?)x.FechaApertura) ?? null;

                DateTime? maxima = await this.UDT.Context.Activos.Where(x => x.Eliminada == false
                && x.ArchivoId == ArchivoId
                && x.EntradaClasificacionId == m.EntradaClasificacionId)
                    .MaxAsync(x => (DateTime?)x.FechaApertura) ?? null;


                if (minima == null) minima = null; ;
                if (maxima == null) maxima = null; ;

                m.FechaMaxCierre = maxima;
                m.FechaMinApertura = minima;
            }

            await this.UDT.Context.SaveChangesAsync();

        }

        private async Task ActualizaConteoEliminados(string ArchivoId)
        {

            var gruposActivos = this.UDT.Context.Activos.Where(x => x.Eliminada == true && x.ArchivoId == ArchivoId).GroupBy
                (x => new { x.CuadroClasificacionId, x.EntradaClasificacionId })
                .Select(y => new
                {
                    CCId = y.Key.CuadroClasificacionId,
                    EntradaID = y.Key.EntradaClasificacionId,
                    TotalActivos = y.Count()
                }).ToList();


            foreach (var m in gruposActivos)
            {
                EstadisticaClasificacionAcervo s = UDT.Context.EstadisticaClasificacionAcervo
                .Where(x => x.CuadroClasificacionId == m.CCId
                   && x.EntradaClasificacionId == m.EntradaID
                   && x.ArchivoId == ArchivoId).SingleOrDefault();

                if (s == null)
                {
                    s = new EstadisticaClasificacionAcervo()
                    {
                        ArchivoId = ArchivoId,
                        ConteoActivos = 0,
                        ConteoActivosEliminados = m.TotalActivos,
                        CuadroClasificacionId = m.CCId,
                        EntradaClasificacionId = m.EntradaID,
                    };
                    UDT.Context.EstadisticaClasificacionAcervo.Add(s);
                }
                else
                {
                    s.ConteoActivosEliminados = m.TotalActivos;
                }
                await UDT.Context.SaveChangesAsync();
            }

        }


        private async Task ActualizaConteoVigentes(string ArchivoId)
        {

            var gruposActivos = this.UDT.Context.Activos.Where(x => x.Eliminada == false && x.ArchivoId == ArchivoId).GroupBy
                (x => new { x.CuadroClasificacionId, x.EntradaClasificacionId })
                .Select(y => new
                {
                    CCId = y.Key.CuadroClasificacionId,
                    EntradaID = y.Key.EntradaClasificacionId,
                    TotalActivos = y.Count()
                }).ToList();


            foreach (var m in gruposActivos)
            {
                EstadisticaClasificacionAcervo s = UDT.Context.EstadisticaClasificacionAcervo
                .Where(x => x.CuadroClasificacionId == m.CCId
                   && x.EntradaClasificacionId == m.EntradaID
                   && x.ArchivoId == ArchivoId).SingleOrDefault();

                if (s == null)
                {
                    s = new EstadisticaClasificacionAcervo()
                    {
                        ArchivoId = ArchivoId,
                        ConteoActivos = m.TotalActivos,
                        ConteoActivosEliminados = 0,
                        CuadroClasificacionId = m.CCId,
                        EntradaClasificacionId = m.EntradaID,
                    };
                    UDT.Context.EstadisticaClasificacionAcervo.Add(s);
                }
                else
                {
                    s.ConteoActivos = m.TotalActivos;
                }
                await UDT.Context.SaveChangesAsync();
            }

        }

    }
}
