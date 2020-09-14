using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.GestionDocumental.Servicios.Reporte;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioEstadisticaClasificacionAcervo : ContextoServicioGestionDocumental, IServicioEstadisticaClasificacionAcervo
    {
        private IRepositorioAsync<EstadisticaClasificacionAcervo> repo;
        private IRepositorioAsync<Activo> repoActivo;
        private IRepositorioAsync<Archivo> repoArchivo;
        private IRepositorioAsync<ElementoClasificacion> repoElementoClasificacion;
        private IRepositorioAsync<EntradaClasificacion> repoEntradaClasificacion;
        private IRepositorioAsync<CuadroClasificacion> repoCuadroClasificacion;
        private List<EntradaClasificacion> ListaEntradas;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private ComunExcel CE;
        private readonly ConfiguracionServidor configuracion;
        private List<ReporteActivos> ListResumen;
        public ServicioEstadisticaClasificacionAcervo
            (IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, IOptions<ConfiguracionServidor> Confi,
           ILogger l)
            : base(proveedorOpciones, l)
        {
            this.configuracion = Confi.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<EstadisticaClasificacionAcervo>(new QueryComposer<EstadisticaClasificacionAcervo>());
            this.repoActivo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
            this.repoArchivo = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoElementoClasificacion = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
            this.repoEntradaClasificacion = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoCuadroClasificacion = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.CE = new ComunExcel();
        }
        public async Task<int> RegistroAñadido(string ArchivoId, string CuadroClasificacionId, string EntradaCuadroId, int Cantidad)
        {
           
            int TotalActivos;
            EstadisticaClasificacionAcervo Estadistica = await this.repo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
            && x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
            && x.EntradaClasificacionId.Equals(EntradaCuadroId, StringComparison.InvariantCultureIgnoreCase)
            );

            if (Estadistica == null)
            {
                Estadistica = new EstadisticaClasificacionAcervo();
                Estadistica.CuadroClasificacionId = CuadroClasificacionId;
                Estadistica.EntradaClasificacionId = EntradaCuadroId;
                Estadistica.ArchivoId = ArchivoId;
                Estadistica.ConteoActivos = Cantidad;
                await this.repo.CrearAsync(Estadistica);
            }
            else
            {
                TotalActivos = Estadistica.ConteoActivos+Cantidad;
                Estadistica.ConteoActivos = TotalActivos;
                UDT.Context.Entry(Estadistica).State = EntityState.Modified;
            }
            UDT.SaveChanges();
            return Estadistica.ConteoActivos;
        }
        public async Task<int> RegistroEliminado(string ArchivoId, string CuadroClasificacionId, string EntradaCuadroId, int Cantidad)
        {

            int TotalActivos;
            EstadisticaClasificacionAcervo eca = await this.repo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
           && x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
           && x.EntradaClasificacionId.Equals(EntradaCuadroId, StringComparison.InvariantCultureIgnoreCase));
            if (eca != null)
            {
                TotalActivos = eca.ConteoActivos - Cantidad;
                if (TotalActivos < 1)
                    TotalActivos = 0;
                eca.ConteoActivos = TotalActivos;
                TotalActivos= eca.ConteoActivosEliminados + Cantidad;
                eca.ConteoActivosEliminados = TotalActivos;
            }

            UDT.Context.Entry(eca).State = EntityState.Modified;
            UDT.SaveChanges();

            return eca.ConteoActivosEliminados;
        }
        public async Task<int> RegistroRestaurado(string ArchivoId, string CuadroClasificacionId, string EntradaCuadroId, int Cantidad)
        {
            int TotalActivos;
            EstadisticaClasificacionAcervo eca = await this.repo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
          && x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
          && x.EntradaClasificacionId.Equals(EntradaCuadroId, StringComparison.InvariantCultureIgnoreCase));

            if (eca != null)
            {
                TotalActivos = eca.ConteoActivos + Cantidad;
                eca.ConteoActivos = TotalActivos;
                TotalActivos = eca.ConteoActivosEliminados - Cantidad;

                if (TotalActivos < 1)
                    TotalActivos = 0;

              eca.ConteoActivosEliminados =TotalActivos;
            }

            UDT.Context.Entry(eca).State = EntityState.Modified;
            UDT.SaveChanges();

            return eca.ConteoActivos;
        }
        public async Task ActualizarConteo(string ArchivoId)
        {
            Activo a = await this.repoActivo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId));
            int TotalActivos=0,TotalActivosEliminados=0;
            string EntradaClasificacionId="";
            var listaActivos =  this.UDT.Context.Activos.GroupBy
                (x => new { x.ArchivoId, x.EntradaClasificacionId,x.Eliminada })
                .Select(y => new { ARchivoID = y.Key.ArchivoId, EntradaID = y.Key.EntradaClasificacionId,
                    estatus=y.Key.Eliminada,TotalActivos = y.ToList().Count() }).ToList();
            
            foreach (var m in listaActivos)
            {
                EntradaClasificacionId = m.EntradaID;
                if (m.estatus)
                    TotalActivosEliminados = Convert.ToInt32(m.TotalActivos);
                else
                    TotalActivos = Convert.ToInt32(m.TotalActivos);
            }
            EntradaClasificacion ec = await this.repoEntradaClasificacion.UnicoAsync(x=>x.Id.Equals(EntradaClasificacionId,StringComparison.InvariantCultureIgnoreCase));
            EstadisticaClasificacionAcervo eca = await this.repo.UnicoAsync(x=>x.CuadroClasificacionId.Equals(ec.CuadroClasifiacionId,StringComparison.InvariantCultureIgnoreCase)
            && x.ArchivoId.Equals(ArchivoId,StringComparison.InvariantCultureIgnoreCase) && x.EntradaClasificacionId.Equals(EntradaClasificacionId,StringComparison.InvariantCultureIgnoreCase));
            if (eca == null)
            {
                EstadisticaClasificacionAcervo e = new EstadisticaClasificacionAcervo();

                e.ArchivoId = ArchivoId;
                e.EntradaClasificacionId = ec.Id;
                e.CuadroClasificacionId = ec.CuadroClasifiacionId;
                e.ConteoActivos = TotalActivos;
                e.ConteoActivosEliminados = TotalActivosEliminados;
                await this.repo.CrearAsync(e);
            }
            else
            {
                eca.ConteoActivos = TotalActivos;
                eca.ConteoActivosEliminados = TotalActivosEliminados;
                UDT.Context.Entry(eca).State = EntityState.Modified;
            }
            
            await this.ReporteEstadisticaArchivoCuadro(ArchivoId,ec.CuadroClasifiacionId,true);
            UDT.SaveChanges();
        }
        public async Task<byte[]> ReporteEstadisticaArchivoCuadro(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros)
        {
            CE.ListEstructuraExcel = new List<Estructuraexcel>();
            this.ListaEntradas = await this.repoEntradaClasificacion.ObtenerAsync(x=>x.Id!=null);

            string Rutacompleta = await CrearRepote(ArchivoId, CuadroClasificacionId, IncluirCeros);

            byte[] b = File.ReadAllBytes(Rutacompleta);

            return b;
        }
        private string ValidarRuta() {
            string ruta = configuracion.ruta_cache_fisico;
            Crearruta(ruta);
            return ruta;
        }
        private void Crearruta(string ruta)
        {
            Directory.CreateDirectory(ruta);
        }
        private async Task<string> CrearRepote(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros)
        {
            string ruta = ValidarRuta();
            string NombreArchivo = $"{Guid.NewGuid()}";

            await NombreArchivoCuadro(ArchivoId, CuadroClasificacionId);
            await CrearCuadroDetalleResumen(ArchivoId, CuadroClasificacionId, IncluirCeros);
            NombreArchivo = CE.CrearArchivoExcel(CE.ListEstructuraExcel, ArchivoId, ruta, null, NombreArchivo);
            
            return NombreArchivo;
        }
        private async Task CrearCuadroDetalleResumen(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros)
        {
            List<EstadisticaClasificacionAcervo> listaAcervo = new List<EstadisticaClasificacionAcervo>();
             ListResumen = new List<ReporteActivos>();

            if (IncluirCeros)
                listaAcervo = await this.repo.ObtenerAsync(x => x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
           && x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase));
            else
                listaAcervo = await this.repo.ObtenerAsync(x => x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
                && x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
                && x.ConteoActivos > 0);

            LlenadoRenumenActivos(ListResumen, listaAcervo);
            CrearreporteActivoLista(ListResumen, listaAcervo);

        }
        private  void LlenadoRenumenActivos(List<ReporteActivos> ListResumen, List<EstadisticaClasificacionAcervo> listaAcervo) 
        {

            foreach (EstadisticaClasificacionAcervo eca in listaAcervo)
            {
                EntradaClasificacion e = ListaEntradas.Find(x=>x.Id==eca.EntradaClasificacionId.Trim());

                ListResumen.Add(new ReporteActivos
                    {
                        Cantidad = eca.ConteoActivos,
                        Nombre = e.Nombre,
                        Clave = e.Clave,
                        NombreCompleto = e.Clave + e.Nombre
                    });
            }

        }
        private void CrearreporteActivoLista(List<ReporteActivos>ListResumen, List<EstadisticaClasificacionAcervo> listaAcervo)
        {
            int renglon = 6;
            int col = 1;
            int Cossecutivo = 0;

            foreach (ReporteActivos reporte in ListResumen.OrderBy(x => x.NombreCompleto))
            {
                CE.CrearCeldas(col, renglon, CE.GetAbecedario(1), $"{ Cossecutivo + 1}");
                CE.CrearCeldas(col, renglon, CE.GetAbecedario(2), reporte.Clave);
                CE.CrearCeldas(col, renglon, CE.GetAbecedario(3), reporte.Nombre);
                CE.CrearCeldas(col, renglon, CE.GetAbecedario(4), reporte.Cantidad.ToString());
                col++;
                renglon++;
            }

            CE.CrearCeldas(4, renglon + 1, CE.GetAbecedario(3), "Total");
            CE.CrearCeldas(4, renglon + 1, CE.GetAbecedario(4), $"{listaAcervo.Sum(x => x.ConteoActivos)}");
            CE.CrearCeldas(4, 2, CE.GetAbecedario(4), "Fecha");
            CE.CrearCeldas(4, 3, CE.GetAbecedario(4), $"{DateTime.Now.ToString("dd/MM/yyyy")}");
        }
        private async Task NombreArchivoCuadro(string ArchivoId, string CuadroClasificacionId)
        {
            int renglon = 2;
            int Col = 1;
            string[] encabezados1 = { "Archivo", "Cuadro Clasificaicón" };
            string[] encabezados2 = { "Consecutivo", "Clave", "Nombre", "Cantidad" };
          renglon= EncabezadosReporteo(encabezados1,renglon,Col,true);
            EncabezadosReporteo(encabezados2,renglon+1,Col,false);
            Archivo a = await this.repoArchivo.UnicoAsync(x => x.Id.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase));
           CE.CrearCeldas(1,2,CE.GetAbecedario(2),a.Nombre);
            CuadroClasificacion cc = await this.repoCuadroClasificacion.UnicoAsync(x => x.Id.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase));
           CE.CrearCeldas(1,3,CE.GetAbecedario(2),cc.Nombre);

        }
        private int EncabezadosReporteo(string[] encabezado, int renglon, int Col, bool TypEcabezado)
        {
            string LetraColumna;
            foreach (string en in encabezado)
            {
                if (TypEcabezado)
                { LetraColumna = CE.GetAbecedario(1);  }
                else
                { LetraColumna = CE.GetAbecedario(Col);  renglon--; }

                CE.CrearCeldas(Col,renglon, LetraColumna, encabezado[Col-1]);
                Col++;
                renglon++;

            }
            renglon++;
            return renglon;
        }
        
      
    }
}
