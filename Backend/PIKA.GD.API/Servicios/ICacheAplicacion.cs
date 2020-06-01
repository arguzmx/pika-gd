using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios
{

    /// <summary>
    /// Alamcena lso valores e estao relevantes para la aplicación
    /// </summary>
    public interface ICacheAplicacion
    {
        ICacheMetadatosAplicacion Metadatos { get; set; }

    }

    /// <summary>
    /// Contiene los valores asociados con plantillas y metadatos
    /// </summary>
    public interface ICacheMetadatosAplicacion
    {
        /// <summary>
        /// Obtiene la definición completa de una plantilla
        /// </summary>
        /// <param name="PlantillaId"></param>
        /// <param name="Modulo"></param>
        /// <returns></returns>
        Task<Plantilla> ObtenerPlantilla(string PlantillaId, string Modulo);
        
        
        /// <summary>
        /// Indica si la plantilla ya ha sido genrada en el backed
        /// </summary>
        /// <param name="PlantillaId"></param>
        /// <returns></returns>
        Task<bool> EsPlantillaGenerada(string PlantillaId);

        /// <summary>
        /// Adiciona un identificaod al estado de las plantillas generadas 
        /// </summary>
        /// <param name="PlantillaId"></param>
        void AdicionaPlantillaGenerada(string PlantillaId);

        /// <summary>
        /// Elimina un identificaod del estado de las plantillas generadas 
        /// </summary>
        /// <param name="PlantillaId"></param>
        void EliminaPlantillaGenerada(string PlantillaId);

        /// <summary>
        /// Elimina tddas las claves de plantillas del cache de plantillas generadas
        /// </summary>
        /// <param name="PlantillaId"></param>
        void EliminaCachePlantillasGeneradas(string PlantillaId);

    }

}
