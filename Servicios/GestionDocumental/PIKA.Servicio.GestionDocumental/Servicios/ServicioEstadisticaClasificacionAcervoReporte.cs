using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.GestionDocumental.Servicios.Reporte;
using RepositorioEntidades;


namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public partial class ServicioEstadisticaClasificacionAcervo
    {
        public async Task<byte[]> ReporteEstadisticaArchivoCuadro(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros)
        {
            ComunExcel CE = new ComunExcel();
            CE.ListEstructuraExcel = new List<Estructuraexcel>();
            //this.ListaEntradas = await this.repoEntradaClasificacion.ObtenerAsync(x => x.Id != null);

            string Rutacompleta = await CrearRepote(ArchivoId, CuadroClasificacionId, IncluirCeros);

            byte[] b = File.ReadAllBytes(Rutacompleta);

            return b;
        }
        private string ValidarRuta()
        {
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
            //NombreArchivo = CE.CrearArchivoExcel(CE.ListEstructuraExcel, ArchivoId, ruta, null, NombreArchivo);

            return NombreArchivo;
        }
        private async Task CrearCuadroDetalleResumen(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros)
        {
            List<EstadisticaClasificacionAcervo> listaAcervo = new List<EstadisticaClasificacionAcervo>();
            List<ReporteActivos>  ListResumen = new List<ReporteActivos>();

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
        private void LlenadoRenumenActivos(List<ReporteActivos> ListResumen, List<EstadisticaClasificacionAcervo> listaAcervo)
        {

            //foreach (EstadisticaClasificacionAcervo eca in listaAcervo)
            //{
            //    EntradaClasificacion e = ListaEntradas.Find(x => x.Id == eca.EntradaClasificacionId.Trim());

            //    ListResumen.Add(new ReporteActivos
            //    {
            //        Cantidad = eca.ConteoActivos,
            //        Nombre = e.Nombre,
            //        Clave = e.Clave,
            //        NombreCompleto = e.Clave + e.Nombre
            //    });
            //}

        }
        private void CrearreporteActivoLista(List<ReporteActivos> ListResumen, List<EstadisticaClasificacionAcervo> listaAcervo)
        {
            //int renglon = 6;
            //int col = 1;
            //int Cossecutivo = 0;

            //foreach (ReporteActivos reporte in ListResumen.OrderBy(x => x.NombreCompleto))
            //{
            //    CE.CrearCeldas(col, renglon, CE.GetAbecedario(1), $"{ Cossecutivo + 1}");
            //    CE.CrearCeldas(col, renglon, CE.GetAbecedario(2), reporte.Clave);
            //    CE.CrearCeldas(col, renglon, CE.GetAbecedario(3), reporte.Nombre);
            //    CE.CrearCeldas(col, renglon, CE.GetAbecedario(4), reporte.Cantidad.ToString());
            //    col++;
            //    renglon++;
            //}

            //CE.CrearCeldas(4, renglon + 1, CE.GetAbecedario(3), "Total");
            //CE.CrearCeldas(4, renglon + 1, CE.GetAbecedario(4), $"{listaAcervo.Sum(x => x.ConteoActivos)}");
            //CE.CrearCeldas(4, 2, CE.GetAbecedario(4), "Fecha");
            //CE.CrearCeldas(4, 3, CE.GetAbecedario(4), $"{DateTime.Now.ToString("dd/MM/yyyy")}");
        }
        private async Task NombreArchivoCuadro(string ArchivoId, string CuadroClasificacionId)
        {
            //int renglon = 2;
            //int Col = 1;
            //string[] encabezados1 = { "Archivo", "Cuadro Clasificaicón" };
            //string[] encabezados2 = { "Consecutivo", "Clave", "Nombre", "Cantidad" };
            //renglon = EncabezadosReporteo(encabezados1, renglon, Col, true);
            //EncabezadosReporteo(encabezados2, renglon + 1, Col, false);
            //Archivo a = await this.repoArchivo.UnicoAsync(x => x.Id.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase));
            //CE.CrearCeldas(1, 2, CE.GetAbecedario(2), a.Nombre);
            //CuadroClasificacion cc = await this.repoCuadroClasificacion.UnicoAsync(x => x.Id.Equals(CuadroClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            //CE.CrearCeldas(1, 3, CE.GetAbecedario(2), cc.Nombre);
            await Task.Delay(1);
        }

        private int EncabezadosReporteo(string[] encabezado, int renglon, int Col, bool TypEcabezado)
        {
            //string LetraColumna;
            //foreach (string en in encabezado)
            //{
            //    if (TypEcabezado)
            //    { LetraColumna = CE.GetAbecedario(1); }
            //    else
            //    { LetraColumna = CE.GetAbecedario(Col); renglon--; }

            //    CE.CrearCeldas(Col, renglon, LetraColumna, encabezado[Col - 1]);
            //    Col++;
            //    renglon++;

            //}
            //renglon++;
            //return renglon;

            return 0;
        }

        public async Task<ICollection<string>> EliminarEstadisticos(int id, string[] ids)
        {
            EstadisticaClasificacionAcervo es = new EstadisticaClasificacionAcervo();
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                switch (id)
                {
                    case 1:
                        es = await this.repo.UnicoAsync(x => x.ArchivoId == Id);
                        break;
                    case 2:
                        es = await this.repo.UnicoAsync(x => x.CuadroClasificacionId == Id);
                        break;
                    case 3:
                        es = await this.repo.UnicoAsync(x => x.EntradaClasificacionId == Id);
                        break;
                }
                if (es != null)
                {
                    await this.repo.Eliminar(es);
                    listaEliminados.Add(es.CuadroClasificacionId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
    }
}
