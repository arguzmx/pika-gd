using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Modelo.Metadatos
{
    public interface IRepositorioMetadatos
    {


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

        /// <summary>
        /// Elmina un elmento único del repositorio
        /// </summary>
        /// <param name="plantilla">Modelo de la plantilla</param>
        /// <param name="id">Identificador del registro a eliminar</param>
        /// <returns></returns>
        Task<bool> Elimina(Plantilla plantilla, string id);

        Task<List<DocumentoPlantilla>> Lista(Plantilla plantilla, string listaId);


        Task<string> Inserta(string tipoid, string id, string tipoOrigenId,
            string origenId, bool esLista, string ListaId,
            Plantilla plantilla, RequestValoresPlantilla valores);

        
        Task<bool> Actualiza(string id, Plantilla plantilla, RequestValoresPlantilla request);

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

    }

}
