using LazyCache;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.EntityFrameworkCore;
using Nest;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido
{
    public class SeguridadContenido: SevicioAuditableBase
    {
        private UnidadDeTrabajo<DbContextContenido> UDT;
        public SeguridadContenido(
            string APP_ID, string MODULO_ID,
            UsuarioAPI usuario,
            ContextoRegistroActividad RegistroActividad,
            List<EventoAuditoriaActivo> EventosActivos,
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            UnidadDeTrabajo<DbContextContenido> UDT,
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


        public async Task<bool> AccesoCachePuntoMontaje(string pmId)
        {
            var cached = await cache.GetAsync<List<string>>($"puntosmontaje-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(pmId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{pmId}") >= 0)
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

                if(cached.Any(x=>x.Equals($"~{pmId}")))
                {
                    return valid;

                } else
                {
                    var roles = ASQLList(usuario.Roles);

                    string sqls = $@"SELECT pm.*  FROM {DbContextContenido.TablaPuntoMontaje} pm INNER JOIN {DbContextContenido.TablaPermisosPuntoMontaje} ppm ON pm.Id = ppm.PuntoMontajeId
WHERE pm.OrigenId = '{RegistroActividad.UnidadOrgId}'  AND ppm.DestinatarioId IN ({roles}) AND ppm.Leer =1";

                    var ca = UDT.Context.PuntosMontaje.FromSqlRaw(sqls);
                    foreach(var pm in ca)
                    {
                        if (!cached.Any(x => x.Equals(pm.Id)))
                        {
                            cached.Add(pm.Id);
                        }
                    }

                    if (cached.Any(x => x.Equals($"{pmId}")))
                    {
                        valid = true;

                    } else
                    {
                        cached.Add($"~{pmId}");
                    }

                    cache.Add($"puntosmontaje-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));

                }
            }

            return valid;
        }

        public async Task<bool> AccesoCacheElementos(string ElementoId)
        {
            var cached = await cache.GetAsync<List<string>>($"elementos-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(ElementoId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{ElementoId}") >= 0)
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

                Elemento el = UDT.Context.Elemento.FirstOrDefault(x => x.Id == ElementoId);
                if(el!= null)
                {
                    var valido = await AccesoCachePuntoMontaje(el.PuntoMontajeId);
                    if (valido)
                    {
                        cached.Add($"~{ElementoId}");
                    } else
                    {
                        cached.Add($"~{ElementoId}");
                    }
                } else
                {
                    cached.Add($"~{ElementoId}");
                }
                cache.Add($"elementos-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }


        public async Task<bool> AccesoCacheVolumen(string VolumenId)
        {
            var cached = await cache.GetAsync<List<string>>($"volumen-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(VolumenId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{VolumenId}") >= 0)
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
                var ca = UDT.Context.Volumen.FirstOrDefault(x => x.Id == VolumenId && x.OrigenId == RegistroActividad.DominioId);
                if (ca != null)
                {
                    if (usuario.Accesos.Any(x => x.OU.Equals(ca.OrigenId)))
                    {
                        cached.Add(VolumenId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{VolumenId}");
                    }
                }
                else
                {
                    cached.Add($"~{VolumenId}");
                }
                cache.Add($"volumen-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }

  
    }
}
