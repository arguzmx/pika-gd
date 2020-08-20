using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Servicios;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Exportar_Importar.Reporte_Transferencia
{
    public class IOTransferencia : ContextoServicioGestionDocumental,
          IServicioInyectable
    {
        private IRepositorioAsync<Transferencia> repoT;
        private IRepositorioAsync<CuadroClasificacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<EntradaClasificacion> RepoEntrda;
        private IRepositorioAsync<TipoArchivo> repoTA;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;
        private IRepositorioAsync<TipoValoracionDocumental> repoTVD;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoTV;
        private IRepositorioAsync<ElementoClasificacion> RepoElemento;
        public IOTransferencia(ILogger<ServicioCuadroClasificacion> Logger, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones)
            : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.RepoEntrda = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoTA = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
            this.repoTD = UDT.ObtenerRepositoryAsync<TipoDisposicionDocumental>(new QueryComposer<TipoDisposicionDocumental>());
            this.repoTVD = UDT.ObtenerRepositoryAsync<TipoValoracionDocumental>(new QueryComposer<TipoValoracionDocumental>());
            this.repoTV = UDT.ObtenerRepositoryAsync<ValoracionEntradaClasificacion>(new QueryComposer<ValoracionEntradaClasificacion>());
            this.repoT = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.RepoElemento = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());

        }

        public async Task<List<string>> ObtenerColumnas(string TransferenciaId, string[] Columnas) 
        {
        
            List<Transferencia> listaT =await this.repoT.ObtenerAsync(x=>x.Id.Equals(TransferenciaId,StringComparison.InvariantCultureIgnoreCase));

            List<string> lista = new List<string>();
            return lista;
        }
        public async Task Obtenetdatos(string TransferenciaId) 
        {
            Transferencia t = await this.repoT.UnicoAsync(x=>x.Id.Equals(TransferenciaId,StringComparison.InvariantCultureIgnoreCase)); 
            //Activo a=
        }
        public async Task<List<Estructuraexcel>> llenadoTabla1(string TransferenciaId, string[] Columnas)
        {
            List<Estructuraexcel> ListEx = new List<Estructuraexcel>();
            string Fila= "Fondo,Transferencia,Archivo Origen,Archivo Destino,Clave";
            int x = 1;
            for (int i = 0; i < 5; i++)
            {
                x++;
                await LlenadoExcel(ListEx, Fila.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray()[i],GetAbecedario(1)+x,1,x);
                
            }
          

            return ListEx;
            
        }

        public async Task<List<Estructuraexcel>> LlenadoExcel(List<Estructuraexcel> lista,string valorRenglon, string Pocisíon,int columna,int renglon) 
        {
            lista.Add(new Estructuraexcel
            {
                 NumeroCulumna=columna,
                 NumeroRenglon=renglon,
                 PosicionColumna=Pocisíon,
                 ValorCelda=valorRenglon
            });
            return lista;
        }
        public string GetAbecedario(int indice)
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
    }
}
