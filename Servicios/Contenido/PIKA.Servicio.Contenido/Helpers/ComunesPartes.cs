using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Helpers
{
    public class ComunesPartes: IComunesPartes
    {
        private IRepositorioAsync<Parte> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        public ComunesPartes(UnidadDeTrabajo<DbContextContenido> UDT)
        {
            this.UDT = UDT;
            this.repo = this.UDT.ObtenerRepositoryAsync<Parte>(new QueryComposer<Parte>());
        }


        /// <summary>
        /// OBtiene una lsita de partes asociada con un elemento y su versión
        /// </summary>
        /// <param name="ElementoId"></param>
        /// <param name="VersionId"></param>
        /// <returns></returns>
        public async Task<List<Parte>> ObtienePartesVersion(string ElementoId, string VersionId)
        {
            var l = await repo.ObtenerAsync(x => x.ElementoId == ElementoId && x.VersionId == VersionId && x.Eliminada == false);
            return l.OrderBy(x => x.Indice).ToList();
        }


        /// <summary>
        /// Crea una lista de partes para la carga de contenidos 
        /// y actualiza las entidades dependientes
        /// </summary>
        /// <param name="partes"></param>
        /// <returns></returns>
        public async Task<List<Parte>> CrearAsync(List<Parte> partes)
        {
            ComunesVersion hver = new ComunesVersion(this.UDT);
            ComunesVolumen hvol = new ComunesVolumen(this.UDT);
            List<Parte> resulatdos = new List<Parte>();

            var grupos = partes.GroupBy(x => new { x.ElementoId, x.VersionId })
                .Select(g => new { ElementoId = g.Key.ElementoId, VersionId = g.Key.VersionId })
                .ToList();

            foreach (var g in grupos)
            {

                
                List<Parte> grupoPartes = partes.Where(x => x.ElementoId == g.ElementoId && x.VersionId == g.VersionId).ToList();

                // Verifica que la versión exista
                Modelo.Contenido.Version version = await hver.Unica(g.VersionId);
                if (version == null) throw new EXNoEncontrado($"Version {g.VersionId}");

                Modelo.Contenido.Volumen volumen = await hvol.Unico(version.VolumenId);
                if (volumen == null) throw new EXNoEncontrado($"Volumenn {version.VolumenId}");

                // Verifica qu eel volumen se encuentre ativo y permita el total del contenito
                if (!(hvol.PermitirInsercion(volumen, grupoPartes.Sum(x => x.LongitudBytes))))
                    throw new ExDatosNoValidos($"Volumen {version.VolumenId}");

                int indice = version.MaxIndicePartes;
                foreach (Parte p in grupoPartes)
                {
                    indice++;
                    if (string.IsNullOrEmpty(p.Id)) p.Id = System.Guid.NewGuid().ToString();
                    p.Eliminada = false;
                    p.Indexada = false;
                    p.Indice = indice;
                    p.ConsecutivoVolumen = 0;
                    p.VolumenId = version.VolumenId;
                    await repo.CrearAsync(p);
                }
                UDT.SaveChanges();

                await hvol.IncrementaTamanoVolumen(volumen.Id, grupoPartes.Sum(x => x.LongitudBytes));
                await hver.RecalcularContenido(version.Id);
                resulatdos.AddRange(grupoPartes);

            }
            
            return resulatdos;
        }

    }
}
