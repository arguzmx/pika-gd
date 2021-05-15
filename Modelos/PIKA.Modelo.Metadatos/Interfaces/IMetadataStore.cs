using PIKA.Modelo.Metadatos.Instancias;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Modelo.Metadatos
{
    public interface IRepositorioMetadatos
    {
        Task<bool> ActualizaDesdePlantilla(Plantilla plantilla);

        /// <summary>
        /// Crea el esapcio  de almacenamiento asociado a la plantilla
        /// </summary>
        /// <param name="plantilla"></param>
        Task<string> CrearIndice(Plantilla plantilla);


        /// <summary>
        /// Actualiza el esapcio  de almacenamiento asociado a la plantilla
        /// </summary>
        /// <param name="plantilla"></param>
        Task<bool> ActualizarIndice(Plantilla plantilla);


        /// <summary>
        /// Elimina el índice asocaidoa la plnatilla
        /// </summary>
        /// <param name="plantilla"></param>
        Task<bool> EliminarIndice(Plantilla plantilla);


        /// <summary>
        /// Obtiene un elemento único del repositorio basado en el Id
        /// </summary>
        /// <param name="plantilla">Modelo de la plantilla</param>
        /// <param name="id">Identificador del registro a recuperar</param>
        /// <returns></returns>
        Task<DocumentoPlantilla> Unico(Plantilla plantilla, string id);

        Task<string> CreaLista(string plantillaId, RequestCrearLista request);

        Task<List<DocumentoPlantilla>> Lista(Plantilla plantilla, string listaId);


        Task<DocumentoPlantilla> Inserta(string tipoOrigenId,
            string origenId, bool esLista, string ListaId,
            Plantilla plantilla, 
            RequestValoresPlantilla valores, string nombreRelacion);

        
        Task<bool> Actualiza(string id, Plantilla plantilla, RequestValoresPlantilla request);

        /// <summary>
        /// Elmina un elmento único del repositorio
        /// </summary>
        /// <param name="plantillaId">Identificador único de la plantilla</param>
        /// <param name="id">Identificador del registro a eliminar</param>
        /// <returns></returns>
        Task<bool> EliminaDocumento(string id, string plantillaId);


        /// <summary>
        /// Elimina todos los documentos de una lista documenos en  una plantilla
        /// </summary>
        /// <param name="id">Identificador único de la lista de documentos</param>
        /// <param name="plantillaId">identificador único de la plantilla</param>
        /// <returns></returns>
        Task<long> EliminaListaDocumentos(string listaId, string plantillaId);

        /// <summary>
        /// Obtiene una lista de elementos de la plantilla
        /// </summary>
        /// <param name="plantilla"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<Paginado<DocumentoPlantilla>> Consulta(Plantilla plantilla, Consulta query);


        /// <summary>
        /// Determina si la plantilla existe registrada en el almacén
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExisteIndice(string id);


        Task<VinculosObjetoPlantilla> ObtieneVinculos(string tipo, string id);


        /// <summary>
        /// Devuleve el numero de docuentos que cumplent una condicio
        /// </summary>
        /// <param name="q"></param>
        /// <param name="plantilla"></param>
        /// <returns></returns>
        Task<long> ContarPorConsulta(Consulta q, Plantilla plantilla, string PuntoMontajeId);

        Task<List<string>> IdsrPorConsulta(Consulta q, Plantilla plantilla, string PuntoMontajeId);

        Task<List<ValoresEntidad>> ConsultaMetadatosPorListaIds(Plantilla plantilla, List<string> Ids);
        
        Task<List<ValoresEntidad>> ConsultaPaginaMetadatosPorListaIds(Plantilla plantilla, List<string> Ids, Consulta q);
    }

}
