using LazyCache;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Seguridad;
using RepositorioEntidades;
using System.Collections.Generic;
using PIKA.Servicio.Metadatos.Data;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PIKA.Servicio.Metadatos
{
    public class SeguridadMetadatos : SevicioAuditableBase
    {
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public SeguridadMetadatos(
            string APP_ID, string MODULO_ID,
            UsuarioAPI usuario,
            ContextoRegistroActividad RegistroActividad,
            List<EventoAuditoriaActivo> EventosActivos,
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            UnidadDeTrabajo<DbContextMetadatos> UDT,
            Dictionary<string, string> Tablas) :
            base(
                APP_ID, MODULO_ID,
                usuario, RegistroActividad, EventosActivos, registroAuditoria,
                cache, Tablas, UDT.Context)
        {
            this.UDT = UDT;
        }


        /// <summary>
        /// Convierte una listas de strings a una lista de SQL para en oeprador IN
        /// </summary>
        /// <param name="l"></param>
        /// <param name="Limitador"></param>
        /// <returns></returns>
        protected string ASQLList(List<string> l, string Limitador = "'")
        {
            string s = "";
            l.ForEach(i =>
            {
                s += $"{Limitador}{i}{Limitador},";
            });

            return s.TrimEnd(',');
        }



        public async Task<bool> AccesoCachePlantillas(string PlantillaId)
        {
            var cached = await cache.GetAsync<List<string>>($"plantillas-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(PlantillaId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{PlantillaId}") >= 0)
                {
                    valid = false;
                }
                else
                {
                    search = true;
                }
            }
            else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.Plantilla.FirstOrDefault(x => x.Id == PlantillaId && x.OrigenId == RegistroActividad.DominioId);
                if (ca != null)
                {
                    cached.Add(PlantillaId);
                    valid = true;
                }
                else
                {
                    cached.Add($"~{PlantillaId}");
                    valid = false;
                }
                cache.Add($"plantillas-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }


    }
}
