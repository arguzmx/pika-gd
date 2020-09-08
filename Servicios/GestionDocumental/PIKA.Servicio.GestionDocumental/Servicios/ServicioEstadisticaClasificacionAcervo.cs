using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
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
        private List<Estructuraexcel> ListResumenAcervo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private readonly ConfiguracionServidor configuracion;
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
        }
        public async Task<int> RegistroAñadido(string ArchivoId, string CuadroClasificacionId, string EntradaCuadroId, int Cantidad)
        {
            int TotalActivos;
            EstadisticaClasificacionAcervo e = new EstadisticaClasificacionAcervo();
            var Estadistica = await this.repo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
            && x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
            && x.EntradaClasificacionId.Equals(EntradaCuadroId, StringComparison.InvariantCultureIgnoreCase)
            );

            if (Estadistica == null)
            {
                e.CuadroClasificacionId = CuadroClasificacionId;
                e.EntradaClasificacionId = EntradaCuadroId;
                e.ArchivoId = ArchivoId;
                e.ConteoActivos = Cantidad;
                await this.repo.CrearAsync(e);
            }
            else
            {
                e = await this.repo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
            && x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
            && x.EntradaClasificacionId.Equals(EntradaCuadroId, StringComparison.InvariantCultureIgnoreCase)
            );
                TotalActivos = Cantidad;
                e.ConteoActivos = TotalActivos;
                UDT.Context.Entry(e).State = EntityState.Modified;
            }
            UDT.SaveChanges();
            this.EliminarDirectorio(configuracion.ruta_cache_fisico);
            return e.ConteoActivos;
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
            this.EliminarDirectorio(configuracion.ruta_cache_fisico);

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
            this.EliminarDirectorio(configuracion.ruta_cache_fisico);

            return eca.ConteoActivos;
        }
        public async Task ActualizarConteo(string ArchivoId)
        {
            EstadisticaClasificacionAcervo eca = new EstadisticaClasificacionAcervo();

            Activo a = await this.repoActivo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId));

            EntradaClasificacion ec = await this.repoEntradaClasificacion.UnicoAsync(x => x.Id.Equals(a.EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            ElementoClasificacion el = await this.repoElementoClasificacion.UnicoAsync(x => x.Id.Equals(ec.ElementoClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            CuadroClasificacion cc = await this.repoCuadroClasificacion.UnicoAsync(x => x.Id.Equals(el.CuadroClasifiacionId, StringComparison.InvariantCultureIgnoreCase));

            List<Activo> lisActivosEliminados = await this.repoActivo.ObtenerAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
                         && x.EntradaClasificacionId.Equals(ec.Id, StringComparison.InvariantCultureIgnoreCase)
                         && x.Eliminada == true);

            List<Activo> lisActivos = await this.repoActivo.ObtenerAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
                         && x.EntradaClasificacionId.Equals(ec.Id, StringComparison.InvariantCultureIgnoreCase)
                         && x.Eliminada == false);

            eca = await this.repo.UnicoAsync(x => x.ArchivoId.Equals(ArchivoId, StringComparison.OrdinalIgnoreCase)
                  && x.CuadroClasificacionId.Equals(cc.Id, StringComparison.InvariantCultureIgnoreCase)
                  && x.EntradaClasificacionId.Equals(ec.Id, StringComparison.InvariantCultureIgnoreCase));

            eca.ArchivoId = ArchivoId;
            eca.EntradaClasificacionId = ec.Id;
            eca.CuadroClasificacionId = cc.Id;
            eca.ConteoActivos = lisActivos.Count;
            eca.ConteoActivosEliminados = lisActivosEliminados.Count;

            if (eca == null)
                await this.repo.CrearAsync(eca);
            else
                UDT.Context.Entry(eca).State = EntityState.Modified;

            UDT.SaveChanges();
            this.EliminarDirectorio(configuracion.ruta_cache_fisico);

        }
        public async Task<byte[]> ReporteEstadisticaArchivoCuadro(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros)
        {
            ListResumenAcervo = new List<Estructuraexcel>();
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
            ComunExcel c = new ComunExcel();
            string ruta = ValidarRuta();
            string NombreArchivo = $"{Guid.NewGuid()}";

            await NombreArchivoCuadro(ArchivoId, CuadroClasificacionId);
            await CrearCuadroDetalleResumen(ArchivoId, CuadroClasificacionId, IncluirCeros);
            NombreArchivo = c.CrearArchivoExcel(ListResumenAcervo, ArchivoId, ruta, null, NombreArchivo);

            return NombreArchivo;
        }
        private async Task CrearCuadroDetalleResumen(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros)
        {
            List<EstadisticaClasificacionAcervo> listaAcervo = new List<EstadisticaClasificacionAcervo>();
            List<ReporteActivos> ListResumen = new List<ReporteActivos>();
         
            if (IncluirCeros)
                listaAcervo = await this.repo.ObtenerAsync(x => x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
           && x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase));
            else
                listaAcervo = await this.repo.ObtenerAsync(x => x.CuadroClasificacionId.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase)
                && x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
                && x.ConteoActivos > 0);

           await LlenadoRenumenActivos(ListResumen, listaAcervo);
            CrearreporteActivoLista(ListResumen, listaAcervo);

        }
        private async Task LlenadoRenumenActivos(List<ReporteActivos> ListResumen, List<EstadisticaClasificacionAcervo> listaAcervo) 
        {
            foreach (EstadisticaClasificacionAcervo eca in listaAcervo)
            {
                EntradaClasificacion e = await ObtieneEntrada(eca.EntradaClasificacionId);
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
                CrearCeldas(col, renglon, GetAbecedario(1), $"{ Cossecutivo + 1}");
                CrearCeldas(col, renglon, GetAbecedario(2), reporte.Clave);
                CrearCeldas(col, renglon, GetAbecedario(3), reporte.Nombre);
                CrearCeldas(col, renglon, GetAbecedario(4), reporte.Cantidad.ToString());
                col++;
                renglon++;
            }

            CrearCeldas(4, renglon + 1, GetAbecedario(3), "Total");
            CrearCeldas(4, renglon + 1, GetAbecedario(4), $"{listaAcervo.Sum(x => x.ConteoActivos)}");
        }
        private async Task NombreArchivoCuadro(string ArchivoId, string CuadroClasificacionId)
        {
            EncabezadosReporteo();

            Archivo a = await this.repoArchivo.UnicoAsync(x => x.Id.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase));
            CrearCeldas(1,2,GetAbecedario(2),a.Nombre);
            CuadroClasificacion cc = await this.repoCuadroClasificacion.UnicoAsync(x => x.Id.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            CrearCeldas(1,3,GetAbecedario(2),cc.Nombre);

        }
        private void EncabezadosReporteo()
        {
            string[] encabezados1 = { "Archivo", "Cuadro Clasificaicón"};
            string[] encabezados2 = { "Consecutivo", "Clave", "Nombre", "Cantidad"};
            int renglon = 2;
            int Col = 1;
            foreach (string encabezado in encabezados1)
            {
                CrearCeldas(Col,renglon, GetAbecedario(1), encabezados1[Col-1]);
                Col++;
                renglon++;
            }
            Col = 1;
            renglon++;
            foreach (string encabezado in encabezados2)
            {
                CrearCeldas(Col, renglon, GetAbecedario(Col), encabezados2[Col - 1]);
                Col++;
            }
            CrearCeldas(4, 2, GetAbecedario(4), "Fecha");
            CrearCeldas(4, 3, GetAbecedario(4), $"{DateTime.Now.ToString("dd/MM/yyyy")}");

        }
        private string GetAbecedario(int indice)
        {
            indice--;
            String col = Convert.ToString((char)('A' + (indice % 26)));
            while (indice >= 26)
            {
                indice = (indice / 26) - 1;
                col = Convert.ToString((char)('A' + (indice % 26))) + col;
            }
            return col;
        }
        private void CrearCeldas(int columna, int renglon, string NombreColumna, string texto)
        {
            ListResumenAcervo.Add(new Estructuraexcel
            {
                NumeroCulumna = columna,
                NumeroRenglon = renglon,
                PosicionColumna = NombreColumna,
                ValorCelda = texto
            });
        }
        private async Task<EntradaClasificacion> ObtieneEntrada(string Entrada) 
        {
            EntradaClasificacion ec =new  EntradaClasificacion();
            ec = await this.repoEntradaClasificacion.UnicoAsync(x=>x.Id.Equals(Entrada,StringComparison.InvariantCultureIgnoreCase));
            return ec;
        }
        private void EliminarDirectorio(string ruta) {
            DirectoryInfo DirecotrioResumen = new DirectoryInfo(ruta);
            foreach (DirectoryInfo dir in DirecotrioResumen.GetDirectories())
                if (Directory.Exists(dir.FullName))
                    Directory.Delete(ruta, true);
        }
    }
}
