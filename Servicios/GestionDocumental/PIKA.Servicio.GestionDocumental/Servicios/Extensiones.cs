using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Nest;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public static class ExtensionesGestionDocumental
    {


        public static string AListaSQL(this List<string>l)
        {
            string s = "";
            l.ForEach(i =>
            {
                s += $"'{i}',";
            });

            return s.TrimEnd(',');
        }
        public static List<PermisosArchivo> PermisosArchivo(this DBContextGestionDocumental c, string UsuarioId, string ArchivoId=null)
      { 
            string sqls = @$"select p.*  from {DBContextGestionDocumental.TablaPermisosArchivo} p
inner join org$rol r on p.DestinatarioId = r.Id
inner join gd$archivo a on p.ArchivoId = a.Id
inner join org$usuarios_rol u on r.Id = u.RolId where
{(ArchivoId != null ? $"p.ArchivoId = '{ArchivoId}' AND" : "")}
 u.ApplicationUserId='{UsuarioId}';";

           return  c.PermisosArchivo.FromSqlRaw(sqls).ToList();
        }

        public static void AdicionaEventoTransferencia(this DBContextGestionDocumental c, string TxId, string EstadoID, string UsuarioId, string comentario = "")
        {
            try
            {
                c.EventosTransferencia.Add(new EventoTransferencia()
                {
                    Comentario = comentario,
                    EstadoTransferenciaId = EstadoID,
                    Fecha = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    TransferenciaId = TxId,
                    UsuarioId = UsuarioId
                });
                c.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                throw;
            }

        }


        public static async Task MueveActivosArchivo(this DBContextGestionDocumental c, List<string> ActivoIds, string ArchivoId)
        {
            string sqls = $"update {DBContextGestionDocumental.TablaActivos} set EnTransferencia = 0, ArchivoId='{ArchivoId}', UbicacionCaja='', UbicacionRack='' where Id in ({ActivoIds.MergeSQLStringList()})";
            await c.Database.ExecuteSqlRawAsync(sqls);
        }

        public static async Task EliminaActivosEnTrasnferencia(this DBContextGestionDocumental c, List<string> ActivoIds)
        {
            string sqls = $"delete from {DBContextGestionDocumental.TablaActivosTransferencia} where ActivoId in ({ActivoIds.MergeSQLStringList()})";
            await c.Database.ExecuteSqlRawAsync(sqls);
        }

        public static async Task ActualizaActivosEnTrasnferencia(this DBContextGestionDocumental c, List<string> ActivoIds, bool EnTransferencia)
        {
            string sqls = $"update {DBContextGestionDocumental.TablaActivos} set EnTransferencia = {(EnTransferencia ? "1": "0")} where Id in ({ActivoIds.MergeSQLStringList()})";
            await c.Database.ExecuteSqlRawAsync(sqls);
        }

        public static async Task ActualizaActivosEnTrasnferencia(this DBContextGestionDocumental c, bool EnTransferencia, string TransferenciaId)
        {
            string sqls = $"update {DBContextGestionDocumental.TablaActivos} set EnTransferencia = {(EnTransferencia ? "1" : "0")} where Id in (select Id  from gd$activotransferencia a where a.TransferenciaId = '{TransferenciaId}')";
            await c.Database.ExecuteSqlRawAsync(sqls);
        }

        public static async Task ActualizaConteoActivosTrasnferencia(this DBContextGestionDocumental c, int delta, string TransferenciaId)
        {
            string sqls = $"update {DBContextGestionDocumental.TablaTransferencias} set CantidadActivos = CantidadActivos + {delta} where Id='{TransferenciaId}'";
            await c.Database.ExecuteSqlRawAsync(sqls);
        }

        public static async Task EstableceConteoActivosTrasnferencia(this DBContextGestionDocumental c, int cantidad, string TransferenciaId)
        {
            string sqls = $"update {DBContextGestionDocumental.TablaTransferencias} set CantidadActivos = {cantidad} where Id='{TransferenciaId}'";
            await c.Database.ExecuteSqlRawAsync(sqls);
        }

        public static string MergeSQLStringList(this List<string> lista)
        {
            StringBuilder Ids = new StringBuilder();
            lista.ForEach(a =>
            {
                Ids.Append($"'{a}',");
            });

            return Ids.ToString().TrimEnd(',');
        }

        public static async Task<List<string>> ActivosValidosTransferencia(this DBContextGestionDocumental c, List<string> ActivosIds, int RangoDias, string ArchivoOrigenId, string CuadroClasificacionId=null, string EntradaClasificacionId=null)
        {
            var archivo = await c.Archivos.Where(t => t.Id == ArchivoOrigenId).FirstOrDefaultAsync();
            var tipoArchivo = await c.TiposArchivo.FirstOrDefaultAsync(x => x.Id == archivo.TipoArchivoId);
            string plantilla = null;

            string retencion = "";
            if (tipoArchivo.Id == TipoArchivo.IDARCHIVO_TRAMITE || tipoArchivo.Tipo == ArchivoTipo.tramite)
            {
                retencion = "FechaRetencionAT";
            }
            else
            {
                retencion = "FechaRetencionAC";
            }

            if (!string.IsNullOrEmpty(retencion))
            {
               

                plantilla =
$@"select a.*  from {DBContextGestionDocumental.TablaActivos} a 
where 
a.ArchivoId = '{archivo.Id}' and a.EnTransferencia=0 and   
a.FechaCierre is not null and a.Reservado=0 and a.Eliminada=0 and a.EnPrestamo=0 and
a.Id in ({ActivosIds.MergeSQLStringList()}) 
{((CuadroClasificacionId != null) ? $"and a.CuadroClasificacionId = '{CuadroClasificacionId}' " : "")} 
{((EntradaClasificacionId != null) ? $"and a.EntradaClasificacionId = '{EntradaClasificacionId}' " : "")} 
and a.{retencion} <= DATE_ADD(now() , INTERVAL {RangoDias} DAY);";
            }

            List<string> transferibles = c.Activos.FromSqlRaw(plantilla).ToList().Select(a => a.Id).ToList();
            return transferibles;

        }


        public static (bool Transferible, string plantilla) GetQueryTransferibles(this Consulta Query)
        {
            bool transferible = false; ;
            string plantilla = null;
            var fTransferible = Query.Filtros.FirstOrDefault(f => f.Propiedad == "PSEUDO_TRANSFERIBLE_TRAMITE");
            string retencion = "";
            if (fTransferible != null)
            {
                transferible = true;
                retencion = "FechaRetencionAT";
            }
            else
            {
                fTransferible = Query.Filtros.FirstOrDefault(f => f.Propiedad == "PSEUDO_TRANSFERIBLE_OTRO");
                if (fTransferible != null)
                {
                    transferible = true;
                    retencion = "FechaRetencionAC";
                }
            }

            if (!string.IsNullOrEmpty(retencion))
            {
                var RangoDias = Query.Filtros.Where(x => x.Propiedad.Equals("RangoDias", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                var qarchivo = Query.Filtros.Where(x => x.Propiedad.Equals("ArchivoId", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                var qcuadro = Query.Filtros.Where(x => x.Propiedad.Equals("CuadroClasificacionId", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                var qentrada = Query.Filtros.Where(x => x.Propiedad.Equals("EntradaClasificacionId", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                var qtexto = Query.Filtros.Where(x => x.Propiedad.Equals("texto", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (qarchivo != null && qtexto != null)
                {

                    plantilla =
$@"select a.*  from {DBContextGestionDocumental.TablaActivos} a 
where 
a.ArchivoId = '{qarchivo.Valor}' and a.EnTransferencia=0
and a.FechaCierre is not null and a.Reservado=0 and a.Eliminada=0 and a.EnPrestamo=0
and (a.Nombre like '%{qtexto.Valor}%' or a.CodigoOptico like '%{qtexto.Valor}%' or a.CodigoElectronico like '%{qtexto.Valor}%')
{((qcuadro != null) ? $"and a.CuadroClasificacionId = '{qcuadro.Valor}' " : "")} 
{((qentrada != null) ? $"and a.EntradaClasificacionId = '{qentrada.Valor}' " : "")} 
and a.{retencion} <= DATE_ADD(now() , INTERVAL {RangoDias.Valor} DAY)  
order by a.Nombre
LIMIT {Query.indice * Query.tamano}, {Query.tamano};";


                }


            }


            return (transferible, plantilla);
        }


        public static UnidadAdministrativaGuiaSimpleArchivo AUnidadGuiaSimple(this UnidadAdministrativaArchivo u , int expedientes)
        {
            return new UnidadAdministrativaGuiaSimpleArchivo()
            {
                ArchivoConcentracionId = u.ArchivoConcentracionId,
                ArchivoHistoricoId = u.ArchivoHistoricoId,
                ArchivoTramiteId = u.ArchivoTramiteId,
                AreaProcedenciaArchivo = u.AreaProcedenciaArchivo,
                Cargo = u.Cargo,
                Domicilio = u.Domicilio,
                Email = u.Email,
                Expedientes = expedientes,
                Id = u.Id,
                Responsable = u.Responsable,
                Telefono = u.Telefono,
                UbicacionFisica = u.UbicacionFisica,
                UnidadAdministrativa = u.UnidadAdministrativa,
                Secciones = new List<SeccionGuiaSimpleArchivo>()
            };
        }

        public static EstadisticaClasificacionAcervo AEstadistica(this Activo a)
        {
            return new EstadisticaClasificacionAcervo()
            {
                ArchivoId = a.ArchivoId,
                ConteoActivos = 0,
                ConteoActivosEliminados = 0,
                CuadroClasificacionId = a.CuadroClasificacionId,
                EntradaClasificacionId = a.EntradaClasificacionId,
                FechaMaxCierre = a.FechaCierre,
                FechaMinApertura = a.FechaApertura,
                UnidadAdministrativaArchivoId = a.UnidadAdministrativaArchivoId
            };
        }

        public static PermisosArchivo Copia(this PermisosArchivo ap)
        {
            return new PermisosArchivo()
            {
                ActualizarAcervo = ap.ActualizarAcervo,
                CrearAcervo = ap.CrearAcervo,
                DestinatarioId = ap.DestinatarioId,
                ElminarAcervo = ap.ElminarAcervo,
                Id = ap.Id,
                LeerAcervo = ap.LeerAcervo,
                ArchivoId = ap.ArchivoId,
                CancelarTrasnferencia = ap.CancelarTrasnferencia,
                CrearTrasnferencia = ap.CrearTrasnferencia,
                EliminarTrasnferencia = ap.EliminarTrasnferencia,
                EnviarTrasnferencia = ap.EnviarTrasnferencia,
                RecibirTrasnferencia = ap.RecibirTrasnferencia
            };
        }

        public static PermisosUnidadAdministrativaArchivo Copia(this PermisosUnidadAdministrativaArchivo ap)
        {
            return new PermisosUnidadAdministrativaArchivo()
            {
                ActualizarAcervo = ap.ActualizarAcervo,
                CrearAcervo = ap.CrearAcervo,
                DestinatarioId = ap.DestinatarioId,
                ElminarAcervo = ap.ElminarAcervo,
                Id = ap.Id,
                LeerAcervo = ap.LeerAcervo,
                UnidadAdministrativaArchivoId = ap.UnidadAdministrativaArchivoId
            };
        }

        public static UnidadAdministrativaArchivo Copia(this UnidadAdministrativaArchivo ap)
        {
            return new UnidadAdministrativaArchivo()
            {
                AreaProcedenciaArchivo = ap.AreaProcedenciaArchivo,
                Cargo = ap.Cargo,
                Domicilio = ap.Domicilio,
                Email = ap.Email,
                Id = ap.Id,
                Responsable = ap.Responsable,
                Telefono = ap.Telefono,
                UbicacionFisica = ap.UbicacionFisica,
                UnidadAdministrativa = ap.UnidadAdministrativa,
                ArchivoTramiteId = ap.ArchivoTramiteId,
                ArchivoConcentracionId = ap.ArchivoConcentracionId,
                ArchivoHistoricoId = ap.ArchivoHistoricoId,
                OrigenId = ap.OrigenId,
                TipoOrigenId = ap.TipoOrigenId
            };
        }
        public static ActivoPrestado CopiaActivoPrestamo(this Activo ap, int indice)
        {
            return new ActivoPrestado()
            {
                Id = ap.Id,
                Nombre = ap.Nombre,
                Asunto = ap.Asunto,
                Reservado = ap.Reservado,
                Confidencial = ap.Confidencial,
                Ampliado = ap.Ampliado,
                ArchivoActual = ap.ArchivoActual,
                ArchivoId = ap.ArchivoId,
                ArchivoOrigenId = ap.ArchivoOrigenId,
                CodigoElectronico = ap.CodigoElectronico,
                CodigoOptico = ap.CodigoOptico,
                CuadroClasificacionId = ap.CuadroClasificacionId,
                ElementoId = ap.ElementoId,
                Eliminada = ap.Eliminada,
                EnPrestamo = ap.EnPrestamo,
                EntradaClasificacionId = ap.EntradaClasificacionId,
                EsElectronico = ap.EsElectronico,
                FechaApertura = ap.FechaApertura,
                FechaCierre = ap.FechaCierre,
                FechaRetencionAC = ap.FechaRetencionAC,
                FechaRetencionAT = ap.FechaRetencionAT,
                TieneContenido = ap.TieneContenido,
                IDunico = ap.IDunico,
                Devuelto = ap.EnPrestamo ? "No" : "Si",
                Indice = indice.ToString()
            };
        }


        public static ActivoPrestamo CopiaActivoPrestamo(this ActivoPrestamo ap)
        {
            return new ActivoPrestamo()
            {
                PrestamoId = ap.PrestamoId,
                ActivoId = ap.ActivoId,
                Devuelto = ap.Devuelto,
                FechaDevolucion = ap.FechaDevolucion
            };
        }

        public static ComentarioPrestamo CopiaComentarioPrestamo(this ComentarioPrestamo cc)
        {
            return new ComentarioPrestamo()
            {
                Id = cc.Id,
                Comentario = cc.Comentario,
                Fecha = cc.Fecha,
                PrestamoId = cc.PrestamoId
            };
        }

    
        #region TRansferencias
        public static Transferencia CopiaTransferencia(this Transferencia t)
        {
            return new Transferencia()
            {
                Id = t.Id,
                Nombre = t.Nombre,
                FechaCreacion = t.FechaCreacion,
                ArchivoOrigen = t.ArchivoOrigen,
                ArchivoDestino = t.ArchivoDestino,
                EstadoTransferenciaId = t.EstadoTransferenciaId,
                UsuarioId = t.UsuarioId
            };
        }

        public static EstadoTransferencia CopiaEstadoTransferencia(this EstadoTransferencia e)
        {
            return new EstadoTransferencia()
            {
                Id = e.Id,
                Nombre = e.Nombre,
            };
        }

        public static EventoTransferencia CopiaEventoTransferencia(this EventoTransferencia e)
        {
            return new EventoTransferencia()
            {
                Id = e.Id,
                TransferenciaId = e.TransferenciaId,
                Fecha = e.Fecha,
                EstadoTransferenciaId = e.EstadoTransferenciaId,
                Comentario = e.Comentario
            };
        }

        public static ComentarioTransferencia CopiaComentarioTransferencia(this ComentarioTransferencia c)
        {
            return new ComentarioTransferencia()
            {
                Id = c.Id,
                TransferenciaId = c.TransferenciaId,
                UsuarioId = c.UsuarioId,
                Fecha = c.Fecha,
                Comentario = c.Comentario,
                Publico = c.Publico,
            };
        }

        public static ActivoTransferencia CopiaActivoTransferencia(this ActivoTransferencia c)
        {
            return new ActivoTransferencia()
            {
                ActivoId = c.ActivoId,
                TransferenciaId = c.TransferenciaId,
            };
        }

 

        #endregion

    }
}
