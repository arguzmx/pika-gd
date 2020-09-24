using Microsoft.EntityFrameworkCore;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido.Helpers
{
    public class  ComunesVersion
    {
        private IRepositorioAsync<Version> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        public ComunesVersion( UnidadDeTrabajo<DbContextContenido> UDT)
        {
            this.UDT = UDT;
            this.repo = this.UDT.ObtenerRepositoryAsync<Version>(new QueryComposer<Version>());
        }

        public async Task<Version> Unica(string Id)
        {
            return await repo.UnicoAsync(x => x.Id == Id);
        } 
 

        public async Task CreaParte(Parte entity)
        {
            var v = await this.repo.UnicoAsync(x => x.Id == entity.VersionId);
            v.ConteoPartes +=1;
            v.TamanoBytes += entity.LongitudBytes;
            v.MaxIndicePartes = entity.Indice;
            UDT.Context.Entry(v).State = EntityState.Modified;
        }


        public async Task ActualizaParte(Parte anterior, Parte Actual)
        {
            var v = await this.repo.UnicoAsync(x => x.Id == anterior.VersionId);
            v.TamanoBytes = v.TamanoBytes - anterior.LongitudBytes + Actual.LongitudBytes;
            UDT.Context.Entry(v).State = EntityState.Modified;
        }


        public async Task EliminaPartes(List<Parte> entity)
        {
            var v = await this.repo.UnicoAsync(x => x.Id == entity[0].VersionId);
            v.TamanoBytes = v.TamanoBytes - entity.Sum(x=>x.LongitudBytes);
            v.ConteoPartes -= entity.Count;
            UDT.Context.Entry(v).State = EntityState.Modified;
        }


        public async Task RestauraPartes(List<Parte> entity)
        {
            var v = await this.repo.UnicoAsync(x => x.Id == entity[0].VersionId);
            v.TamanoBytes = v.TamanoBytes + entity.Sum(x=>x.LongitudBytes);
            v.ConteoPartes += entity.Count;
            UDT.Context.Entry(v).State = EntityState.Modified;
        }

        /// <summary>
        /// Actualiza las estad´siticas de la version
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public async Task RecalcularContenido(string versionId)
        {

            var v = await this.repo.UnicoAsync(x => x.Id == versionId, null, y => y.Include(z=>z.Partes));
            if (v != null)
            {
                if (v.Partes != null)
                {
                    v.TamanoBytes = v.Partes.Sum(x => x.LongitudBytes);
                    v.ConteoPartes += v.Partes.Count;
                    v.MaxIndicePartes = v.Partes.Max(x => x.Indice) + 1;
                } else
                {
                    v.TamanoBytes = 0;
                    v.ConteoPartes += 0;
                    v.MaxIndicePartes = 0;
                }
                UDT.Context.Entry(v).State = EntityState.Modified;
                UDT.SaveChanges();
            }
        }

    }
}
