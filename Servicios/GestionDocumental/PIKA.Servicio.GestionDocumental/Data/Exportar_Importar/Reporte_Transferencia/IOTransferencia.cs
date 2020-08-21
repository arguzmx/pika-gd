using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Servicios;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Exportar_Importar.Reporte_Transferencia
{
  public  class IOTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable
    {
        private IRepositorioAsync<Transferencia> repoT;
        private IRepositorioAsync<CuadroClasificacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<EntradaClasificacion> RepoEntrda;
        private IRepositorioAsync<Activo> repoAc;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;
        private IRepositorioAsync<TipoValoracionDocumental> repoTVD;
        private IRepositorioAsync<Archivo> repoAr;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoTV;
        private IRepositorioAsync<ElementoClasificacion> RepoElemento;

        public IOTransferencia(ILogger<ServicioCuadroClasificacion> Logger, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones)
            : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.RepoEntrda = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoAc = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
            this.repoTD = UDT.ObtenerRepositoryAsync<TipoDisposicionDocumental>(new QueryComposer<TipoDisposicionDocumental>());
            this.repoTVD = UDT.ObtenerRepositoryAsync<TipoValoracionDocumental>(new QueryComposer<TipoValoracionDocumental>());
            this.repoTV = UDT.ObtenerRepositoryAsync<ValoracionEntradaClasificacion>(new QueryComposer<ValoracionEntradaClasificacion>());
            this.repoT = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.repoAr = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.RepoElemento = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());


        }

        public async Task<List<string>> ObtenerColumnas(string TransferenciaId, string[] Columnas)
        {

            List<Transferencia> listaT = await this.repoT.ObtenerAsync(x => x.Id.Equals(TransferenciaId, StringComparison.InvariantCultureIgnoreCase));

            List<string> lista = new List<string>();
            return lista;
        }
        public async Task<string> Obtenetdatos(string TransferenciaId, string[] Columnas,string ruta,string separador)
        {
            CrearDocumento c = new CrearDocumento();
            List<Estructuraexcel> ListEx = new List<Estructuraexcel>();

            Transferencia t = await this.repoT.UnicoAsync(x => x.Id.Equals(TransferenciaId, StringComparison.InvariantCultureIgnoreCase));
            Activo ac1 = await this.repoAc.UnicoAsync(x => x.ArchivoId.Equals(t.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase));
            EntradaClasificacion ec1 = await this.RepoEntrda.UnicoAsync(x => x.Id.Equals(ac1.EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            ElementoClasificacion EC1 = await this.RepoElemento.UnicoAsync(x => x.Id.Equals(ec1.ElementoClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            CuadroClasificacion cc = await this.repo.UnicoAsync(x => x.Id.Equals(EC1.CuadroClasifiacionId, StringComparison.InvariantCultureIgnoreCase));

   

            //List<EntradaClasificacion> liEn = await this.RepoEntrda.ObtenerAsync(x=>x.Id.Contains());
            string name = await c.CrearArchivoExcel(await LlenadoTabla1(ListEx, TransferenciaId, Columnas,t.ArchivoOrigenId,t,ac1), TransferenciaId, ruta, separador, ec1.Clave);
            ListEx = new List<Estructuraexcel>();
             ac1 = await this.repoAc.UnicoAsync(x => x.ArchivoId.Equals(t.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase));
             ec1 = await this.RepoEntrda.UnicoAsync(x => x.Id.Equals(ac1.EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));
             EC1 = await this.RepoElemento.UnicoAsync(x => x.Id.Equals(ec1.ElementoClasificacionId, StringComparison.InvariantCultureIgnoreCase));
             cc = await this.repo.UnicoAsync(x => x.Id.Equals(EC1.CuadroClasifiacionId, StringComparison.InvariantCultureIgnoreCase));
            await c.CreaArchivoExistente(await LlenadoTabla1(ListEx, TransferenciaId, Columnas, t.ArchivoDestinoId,t, ac1), TransferenciaId, name, ec1.Clave);



           
            return name ;
        }
        public async Task<List<Estructuraexcel>> LlenadoTabla1(List<Estructuraexcel>li,string TransferenciaId, string[] Columnas,string ArchivoID, Transferencia t,Activo ac)
        {
            string Fila = "Fondo,Transferencia,Archivo Origen,Archivo Destino,Clave";
            int x = 1;
            for (int i = 0; i < 5; i++)
            {
                
                await LlenadoExcel(li, Fila.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray()[i], GetAbecedario(1), 1, x+1);
                await llenadoTablaDetalleTransferencia(i,li,x+1,Columnas, ArchivoID,t,ac);
                x++;
            }
            
            return li;

        }
        public async Task<List<Estructuraexcel>> llenadoTablaDetalleTransferencia(int indice,List<Estructuraexcel>li, int renglon,string[] col,string ArchivoID,Transferencia t, Activo ac1) 
        {
            switch (indice)
            {
                case 0:
                     ac1 = await this.repoAc.UnicoAsync(x => x.ArchivoId.Equals(ArchivoID, StringComparison.InvariantCultureIgnoreCase));
                    EntradaClasificacion ec1 = await this.RepoEntrda.UnicoAsync(x => x.Id.Equals(ac1.EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));
                    ElementoClasificacion EC1 = await this.RepoElemento.UnicoAsync(x => x.Id.Equals(ec1.ElementoClasificacionId,StringComparison.InvariantCultureIgnoreCase));
                    CuadroClasificacion cc = await this.repo.UnicoAsync(x=>x.Id.Equals(EC1.CuadroClasifiacionId,StringComparison.InvariantCultureIgnoreCase));
                    List<Activo> liA = await this.repoAc.ObtenerAsync(x => x.EntradaClasificacionId.Equals(ac1.EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase), y => y.OrderBy(z => z.Nombre));
                    await LlenadoExcel(li, cc.Nombre, GetAbecedario(2), 2, renglon);
                    await LlenadoExcel(li, "Total de Registros", GetAbecedario(5), 1, renglon);
                    await LlenadoExcel(li, liA.Count().ToString(), GetAbecedario(6), 1, renglon);

                    break;
                case 1:
                    await LlenadoExcel(li, t.Nombre, GetAbecedario(2), 2, renglon);
                    break;
                case 2:
                    Archivo AO = await this.repoAr.UnicoAsync(x=>x.Id.Equals(t.ArchivoOrigenId,StringComparison.InvariantCultureIgnoreCase));
                    await LlenadoExcel(li, AO.Nombre, GetAbecedario(2), 2, renglon);
                    break;
                case 3:
                    Archivo AD = await this.repoAr.UnicoAsync(x => x.Id.Equals(t.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase));
                    await LlenadoExcel(li, AD.Nombre, GetAbecedario(2), 2, renglon);
                    break;
                case 4:
                    Activo ac = await this.repoAc.UnicoAsync(x=>x.ArchivoId.Equals(ArchivoID,StringComparison.InvariantCultureIgnoreCase));
                    EntradaClasificacion ec = await this.RepoEntrda.UnicoAsync(x=>x.Id.Equals(ac.EntradaClasificacionId,StringComparison.InvariantCultureIgnoreCase));
                    await LlenadoExcel(li, ec.Clave, GetAbecedario(2), 2, renglon);
                    await LlenadoExcel(li, ec.Nombre, GetAbecedario(3), 2, renglon);
                    await ColumnasDetalle(li, col, renglon, ec.Id,ac1);

                    break;
                default:
                    break;
            }
            return li;
        }
        public async Task<List<Estructuraexcel>> ColumnasDetalle(List<Estructuraexcel> li,string[] Columnas, int renglon,string EntradaId, Activo ac) 
        {
            int columna = 2;
            List<Activo> liA = await this.repoAc.ObtenerAsync(x=>x.EntradaClasificacionId.Equals(ac.EntradaClasificacionId,StringComparison.InvariantCultureIgnoreCase),y => y.OrderBy(z => z.Nombre));
            renglon = renglon + 3;
            await LlenadoExcel(li, "Consecutivo", GetAbecedario(columna-1), columna-1, renglon);

            foreach (string col in Columnas)
                {
                   
                await LlenadoExcel(li, col, GetAbecedario(columna), columna, renglon);
                    await llenado(col, li, col, GetAbecedario(columna), columna, renglon,liA);

                columna++;
                }
          
            
          
            
            return li;
        }

        private async Task llenado(string tipoColumna, List<Estructuraexcel> li, string valorRenglon, string Pocisíon, int columna, int renglon, List<Activo> liA) 
        {
            int x = 1;

            foreach (Activo act in liA)
            { 
                renglon = renglon + 1;
                EntradaClasificacion ec = await this.RepoEntrda.UnicoAsync(x => x.Id.Equals(act.EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));
                await LlenadoExcel(li, x.ToString(), GetAbecedario(1), 1, renglon);
                switch (tipoColumna)
                {
                    case "EntradaClasificacion.Clave":
                        await LlenadoExcel(li, ec.Clave, GetAbecedario(columna), columna, renglon);
                        break;
                    case "EntradaClasificacion.Nombre":
                        await LlenadoExcel(li, ec.Nombre, GetAbecedario(columna), columna, renglon);
                        break;
                    case "Nombre":
                        await LlenadoExcel(li, act.Nombre, GetAbecedario(columna), columna, renglon);
                        break;
                    case "Asunto":
                        await LlenadoExcel(li, act.Asunto, GetAbecedario(columna), columna, renglon);
                        break;
                    case "FechaApertura":
                        await LlenadoExcel(li, act.FechaApertura.ToShortDateString(), GetAbecedario(columna), columna, renglon);
                        break;
                    case "FechaCierre":
                        string fecha = "";
                        if (!String.IsNullOrEmpty(act.FechaCierre.ToString()))
                            fecha = act.FechaCierre.ToString();
                        await LlenadoExcel(li, fecha, GetAbecedario(columna), columna, renglon);
                        break;
                    case "CodigoOptico":
                        await LlenadoExcel(li, act.CodigoOptico, GetAbecedario(columna), columna, renglon);
                        break;
                    case "CodigoElectronico":
                        await LlenadoExcel(li, act.CodigoElectronico, GetAbecedario(columna), columna, renglon);
                        break;
                    case "Reservado":
                        await LlenadoExcel(li, Convert.ToInt32(act.Reservado).ToString(), GetAbecedario(columna), columna, renglon);
                        break;
                    case "Confidencial":
                        await LlenadoExcel(li,Convert.ToInt32(act.Confidencial).ToString(), GetAbecedario(columna), columna, renglon);
                        break;
                    case "Ampliado":
                        await LlenadoExcel(li, Convert.ToInt32(act.Ampliado).ToString(), GetAbecedario(columna), columna, renglon);
                        break;
                    default:
                        break;
                }

                x++;
            }
            
        }
        private async Task<List<Estructuraexcel>> LlenarDetalle(List<Estructuraexcel> li) 
        {
            return li;
        }
        public async Task<List<Estructuraexcel>> LlenadoExcel(List<Estructuraexcel> lista, string valorRenglon, string Pocisíon, int columna, int renglon)
        {
            lista.Add(new Estructuraexcel
            {
                NumeroCulumna = columna,
                NumeroRenglon = renglon,
                PosicionColumna = Pocisíon,
                ValorCelda = valorRenglon
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