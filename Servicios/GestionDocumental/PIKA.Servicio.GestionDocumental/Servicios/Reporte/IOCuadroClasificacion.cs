﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Servicios;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental
{
    public class IOCuadroClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable
    {
        private IRepositorioAsync<CuadroClasificacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<EntradaClasificacion> RepoEntrda;
        private IRepositorioAsync<TipoArchivo> repoTA;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;
        private IRepositorioAsync<TipoValoracionDocumental> repoTVD;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoTV;
        private IRepositorioAsync<ElementoClasificacion> RepoElemento;
        public IOCuadroClasificacion(IRegistroAuditoria regisstroAuditoria, ILogger<ServicioLog> Logger, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones) 
            : base(regisstroAuditoria, proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.RepoEntrda = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoTA = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
            this.repoTD = UDT.ObtenerRepositoryAsync<TipoDisposicionDocumental>(new QueryComposer<TipoDisposicionDocumental>());
            this.repoTVD = UDT.ObtenerRepositoryAsync<TipoValoracionDocumental>(new QueryComposer<TipoValoracionDocumental>());
            this.repoTV = UDT.ObtenerRepositoryAsync<ValoracionEntradaClasificacion>(new QueryComposer<ValoracionEntradaClasificacion>());
            this.RepoElemento = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
           

        }

       
      /// <summary>
      /// Crea el archivo xml
      /// </summary>
      /// <param name="CuadroClasificacionId"></param>
      /// <returns></returns>
        public async Task<byte[]> ExportarCuadroCalsificacionExcel(string CuadroClasificacionId,string ruta,string separador)
        {
            CrearDocumento CD = new CrearDocumento();
            List<Estructuraexcel> listExport = new List<Estructuraexcel>();

            string Nombre =   await LlenadoCuadroClasificacion(CuadroClasificacionId.Trim(), listExport);

            string fileName= CD.CrearArchivo(listExport, CuadroClasificacionId.Trim(), ruta, separador, Nombre);

            byte[] array = File.ReadAllBytes(fileName);
            return array;


        }
        /// <summary>
        /// Elemina la ruta del cuadro clasificacion siempre y cuando se haga un cambbio a la estructura del cuadro clasificación
        /// </summary>
        /// <param name="CuadroClasificacionId">Identificador unico </param>
        /// <param name="ruta">Ruta fisica</param>
        /// <param name="separador">separador de ruta</param>
        /// <returns></returns>
        public async Task EliminarCuadroCalsificacionExcel(string CuadroClasificacionId, string ruta, string separador) 
        {
            CuadroClasificacion cc = await this.repo.UnicoAsync(x=>x.Id.Equals(CuadroClasificacionId,StringComparison.InvariantCultureIgnoreCase));
            try
            {
                Directory.Delete($"{ruta}{CuadroClasificacionId}", true);
               
            }
            catch (Exception ex)
            {
            }
            

        }
  
        /// <summary>
        /// llenado del cuadro clasidicación
        /// </summary>
        /// <param name="CuadroClasificacionId"></param>
        /// <param name="listExport"></param>
        /// <returns></returns>
        private async Task<string> LlenadoCuadroClasificacion(string CuadroClasificacionId, List<Estructuraexcel> listExport) {
            int columna = 1;
            int Row = 2;
            string nombre="";
            

            CuadroClasificacion cuadroclasificacion = await this.repo.UnicoAsync(x=> x.Id.Equals(CuadroClasificacionId,StringComparison.InvariantCultureIgnoreCase));
            List<RegionElementoClasificacion> lr1 = new List<RegionElementoClasificacion>();
            if (cuadroclasificacion != null)
            {
                nombre = cuadroclasificacion.Nombre;
                LlenadoEstructuraExcel(listExport, columna, Row, $" {cuadroclasificacion.Nombre} ");
                Row = Row + 3;

                List <ElementoClasificacion> el = await this.RepoElemento.ObtenerAsync(x=>x.CuadroClasifiacionId.Equals(CuadroClasificacionId,StringComparison.InvariantCultureIgnoreCase) && x.EsRaiz==true);
                foreach (ElementoClasificacion e in el)
                {
                    LlenadoRegion1(lr1, columna, $" {e.Clave} {e.Nombre} ", e.CuadroClasifiacionId, e.Id);
                    await ObtnerHijosElmentos(lr1, columna, cuadroclasificacion.Id, e.Id);

                }
            }

           await LlenadoRegion2y3(lr1, listExport);

            return nombre;
        }
        /// <summary>
        /// Llenado de la region 2 y 3
        /// </summary>
        /// <param name="lr1">lista de elementos </param>
        /// <param name="listExport">lista del llenado por celdas</param>
        /// <returns></returns>
        private async Task LlenadoRegion2y3(List<RegionElementoClasificacion> lr1, List<Estructuraexcel> listExport) 
        {
            int contador = 4;
            foreach (RegionElementoClasificacion r in lr1)
            {
                LlenadoEstructuraExcel(listExport, r.Nivel, contador, $" {r.NombreClave} ");
                List<EntradaClasificacion> lisentrada = await this.RepoEntrda.ObtenerAsync(x => x.ElementoClasificacionId.Equals
                (r.ElementoId, StringComparison.InvariantCultureIgnoreCase));
                if (lisentrada.Count() > 0)
                    await encabezados(listExport, lr1.Max(x => x.Nivel));
                foreach (EntradaClasificacion entradas in lisentrada)
                {
                    await InsertaValoresCuadro2(listExport, entradas, lr1.Max(x => x.Nivel), contador);
                    contador++;
                }
                contador++;
            }
        }

        /// <summary>
        /// Crea los Encabezados de la region 2 y 3 de las entidades EntradaCuadroClasificación y Tipo valoración
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private async Task encabezados(List<Estructuraexcel> exp, int c)
        {
            int row = 2;
            c = c + 1;
            LlenadoEstructuraExcel(exp, c, row, "Clave");
            List<TipoValoracionDocumental> list = await this.repoTVD.ObtenerAsync(x => x.Id != null, include: null);

       
            for (int i = 1; i < 4; i++)
            {
                c = c + 1;
                LlenadoEstructuraExcel(exp, c, row, ValorEncabezado(i));
            }

            foreach (TipoValoracionDocumental tp in list)
            {
                c = c + 1;
                LlenadoEstructuraExcel(exp, c, row, tp.Nombre);

            }

        }
        /// <summary>
        /// /Esta funsión retorna un string que será el nombre que llevará el encabezado de la región 2
        /// </summary>
        /// <param name="indice">Identificar de la celda para cada columna de la region 2 del excel</param>
        /// <returns></returns>
        private string ValorEncabezado(int indice) 
        {
            string ListEncabezado = "AT,AC,Tipo Disposición";
            string text="";
            switch (indice)
            {
                case 1:
                    text = ListEncabezado.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray()[0];
                    break;
                case 2:
                    text = ListEncabezado.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray()[1];
                    break;
                case 3:
                    text = ListEncabezado.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray()[2];
                    break;
              
            }
            return text;
        }
        /// <summary>
        /// Esta función esta diseñada para llenar una lista por cada una de las celdas que serán imprimidas en el archivo
        /// </summary>
        /// <param name="exp">lista de estructura del excel</param>
        /// <param name="NumeroCulumna">Numero de Columna</param>
        /// <param name="NumeroRenglon">numero de Renglón</param>
        /// <param name="PosicionColumna">Posición de la columna</param>
        /// <param name="ValorCelda">Valor de la </param>
        private void LlenadoEstructuraExcel(List<Estructuraexcel> exp, int NumeroCulumna,int NumeroRenglon, string ValorCelda) 
        {
            exp.Add(new Estructuraexcel
            {
                NumeroCulumna = NumeroCulumna,
                NumeroRenglon = NumeroRenglon,
                PosicionColumna = $"{GetAbecedario(NumeroCulumna)}",
                ValorCelda = ValorCelda

            });
        }
        /// <summary>
        /// Llena la lista de estructura del excel para el llenado de celdas
        /// </summary>
        /// <param name="exp">lista de datos</param>
        /// <param name="EntradasClasificion">entidad de entrada clasificacion</param>
        /// <param name="IndiceColumna">Indice de la columna</param>
        /// <param name="NumeroFila">Numero de Fila para la celda</param>
        /// <param name="PosicionColumna"></param>
        /// <returns></returns>
        private async Task InsertaValoresCuadro2(List<Estructuraexcel> exp, EntradaClasificacion EntradasClasificion, int IndiceColumna, int NumeroFila)
        {
            string nombreclave = $"{ EntradasClasificion.Clave}{ EntradasClasificion.Nombre}";
            List<TipoValoracionDocumental> list = await this.repoTVD.ObtenerAsync(x => x.Id != null, include: null);
            IndiceColumna = IndiceColumna + 1;
            LlenadoEstructuraExcel(exp, IndiceColumna, NumeroFila, nombreclave);
           
         
            for (int i = 1; i < 4; i++)
            {

                IndiceColumna = IndiceColumna + 1;
                if (i == 1)
                { nombreclave = EntradasClasificion.VigenciaTramite.ToString();
                    LlenadoEstructuraExcel(exp, IndiceColumna, NumeroFila, nombreclave);
                }
                if (i == 2)
                { nombreclave = EntradasClasificion.VigenciaConcentracion.ToString();
                    LlenadoEstructuraExcel(exp, IndiceColumna, NumeroFila, nombreclave);
                }
                if (i == 3)
                {
                    TipoDisposicionDocumental tp = await this.repoTD.UnicoAsync(x => x.Id == EntradasClasificion.TipoDisposicionDocumentalId);
                    if (tp != null)
                    {
                        nombreclave =  tp.Nombre;
                    }
                    else {
                        nombreclave = " " ;
                    }
                    LlenadoEstructuraExcel(exp, IndiceColumna, NumeroFila, nombreclave);

                }


            }

            foreach (TipoValoracionDocumental tp in list)
            {
                IndiceColumna = IndiceColumna + 1;
                List<ValoracionEntradaClasificacion> vl = await this.repoTV.ObtenerAsync(x => x.TipoValoracionDocumentalId.Equals(tp.Id) && x.EntradaClasificacionId.Equals(EntradasClasificion.Id));
                if (vl.Count() > 0)
                {
                    LlenadoEstructuraExcel(exp, IndiceColumna, NumeroFila, "X");
                }

            }
        }

        /// <summary>
        /// devuelve un string con la lectra del alfabeto que se obtiene el indice recibido
        /// </summary>
        /// <param name="indice">Identificar del Indice de la posición del alfabeto</param>
        /// <returns></returns>
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
        /// <summary>
        /// Obtiene todos los niveles y los hijos de cada elemento
        /// </summary>
        /// <param name="lr1">lista de región </param>
        /// <param name="Indicecolumna">Identificador del indice de la columna</param>
        /// <param name="CuadroClasificacionId">Identificador único del cuadro Clasificación</param>
        /// <param name="ElementoClasificacionId">Identificador único del elemento clasificación</param>
        /// <returns></returns>
        private async Task ObtnerHijosElmentos(List<RegionElementoClasificacion> lr1, int Indicecolumna, string CuadroClasificacionId, string? ElementoClasificacionId)
        {
            if (string.IsNullOrEmpty(ElementoClasificacionId))
            {
                Indicecolumna = Indicecolumna - 1;
                
            }

            List<ElementoClasificacion> ListaElemntosHijos = new List<ElementoClasificacion>();
            ListaElemntosHijos = await this.RepoElemento.ObtenerAsync(x=>x.ElementoClasificacionId.Equals(ElementoClasificacionId,StringComparison.InvariantCulture)
            &&x.CuadroClasifiacionId.Equals(CuadroClasificacionId,StringComparison.InvariantCulture));
            if (ListaElemntosHijos.Count() > 0)
            { Indicecolumna++;  }
            foreach (ElementoClasificacion hijos in ListaElemntosHijos)
            {
                LlenadoRegion1(lr1,Indicecolumna, $" {hijos.Clave} {hijos.Nombre} ",hijos.CuadroClasifiacionId,hijos.Id);
                await ObtnerHijosElmentos(lr1, Indicecolumna, hijos.CuadroClasifiacionId, hijos.Id);
            }
            
            
           

        }
        /// <summary>
        /// Se llena la clase Región1 para obtener todos los niveles o columnas que se deben considerar.
        /// </summary>
        /// <param name="r1">lista de la región</param>
        /// <param name="columna">Numero de columna por hijo encontrado si este tiene hijos le agrega una columna por cada padre que encuentre</param>
        /// <param name="nombreClave">Parametro donde se concatena el nombre y la clave del elemento</param>
        /// <param name="CuadroClasificacionId">Identificador único del cuadro clasificación</param>
        /// <param name="ElementoClasificacionId">Identificador Unico del elemento clasificacion</param>
        /// <returns></returns>
        private List<RegionElementoClasificacion> LlenadoRegion1(List<RegionElementoClasificacion> r1, int columna,string nombreClave,string CuadroClasificacionId,string ElementoClasificacionId ) {
            r1.Add(new RegionElementoClasificacion
            {
                Nivel = columna,
                NombreClave = nombreClave ,
                CuadroClasificacionId = CuadroClasificacionId,
                ElementoId = ElementoClasificacionId
            });
            return r1;
        }
       
    }
}
