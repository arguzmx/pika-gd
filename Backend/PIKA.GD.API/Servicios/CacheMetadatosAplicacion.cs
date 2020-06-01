using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Interfaces;
using Serilog.Core;
using Serilog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios
{
    /// <summary>
    /// Contiene los valores asociados con plantillas y metadatos
    /// </summary>
    public class CacheMetadatosAplicacion : ICacheMetadatosAplicacion
    {

        private const string REPOMETADATOS = "repom";

        private readonly IServicioPlantilla servicioPlantilla;
        private readonly IAPICache<Plantilla> cachePlantilla;
        private readonly IAPICache<string> cacheClaves;
        public CacheMetadatosAplicacion(
            IServicioPlantilla servicioPlantilla,
            IAPICache<Plantilla> cachePlantilla,
            IAPICache<string> cacheClaves
            )
        {
            this.servicioPlantilla = servicioPlantilla;
            this.cachePlantilla = cachePlantilla;
            this.cacheClaves = cacheClaves;
        }

        /// <summary>
        /// Obtiene la definición completa de una plantilla
        /// </summary>
        /// <param name="PlantillaId"></param>
        /// <param name="Modulo"></param>
        /// <returns></returns>
        public async Task<Plantilla> ObtenerPlantilla(string PlantillaId, string Modulo)
        {

            Plantilla plantilla = await cachePlantilla.Obtiene(PlantillaId, Modulo).ConfigureAwait(false);
            if (plantilla == null)
            {
                plantilla = await servicioPlantilla.UnicoAsync(
                    predicado: x => x.Id == PlantillaId,
                    incluir:
                    y => y.Include(z => z.Propiedades).ThenInclude(z => z.TipoDato)
                    .Include(z => z.Propiedades).ThenInclude(z => z.ValidadorNumero)
                    .Include(z => z.Propiedades).ThenInclude(z => z.ValidadorTexto)
                    .Include(z => z.Propiedades).ThenInclude(z => z.AtributoTabla)
                     ).ConfigureAwait(false);

                if (plantilla != null)
                {
                    await cachePlantilla.Inserta(PlantillaId, Modulo, plantilla, new TimeSpan(1, 0, 0)).ConfigureAwait(false);
                }

            }
            return plantilla;
        }


        /// <summary>
        /// Elimina tddas las claves de plantillas del cache de plantillas generadas
        /// </summary>
        /// <param name="PlantillaId"></param>
        public void EliminaCachePlantillasGeneradas(string PlantillaId)
        {
            cacheClaves.EliminaSiContiene(PlantillaId);
        }


        /// <summary>
        /// Elimina un identificaod del estado de las plantillas generadas 
        /// </summary>
        /// <param name="PlantillaId"></param>
        public void EliminaPlantillaGenerada(string PlantillaId)
        {
            cacheClaves.Elimina(PlantillaId, REPOMETADATOS); 
        }

        /// <summary>
        /// Indica si la plantilla ya ha sido genrada en el backed
        /// </summary>
        /// <param name="PlantillaId"></param>
        /// <returns></returns>
        public async Task<bool> EsPlantillaGenerada(string PlantillaId)
        {
            if ((await cacheClaves.Obtiene(PlantillaId, REPOMETADATOS).ConfigureAwait(false)) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adiciona un identificaod al estado de las plantillas generadas 
        /// </summary>
        /// <param name="PlantillaId"></param>
        public void AdicionaPlantillaGenerada(string PlantillaId)
        {
            cacheClaves.Inserta(PlantillaId, REPOMETADATOS, PlantillaId, new TimeSpan(24, 0, 0));
        }

    }
}
