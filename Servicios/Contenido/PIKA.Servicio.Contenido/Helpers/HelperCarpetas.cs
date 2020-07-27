using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Helpers
{
    public class  HelperCarpetas
    {
        private IRepositorioAsync<Carpeta> repo;
        public HelperCarpetas(IRepositorioAsync<Carpeta> repo)
        {
            this.repo = repo;
        }

        /// <summary>
        /// Identifica si la caprtea destino no es hija de la carpeta a amover
        /// </summary>
        /// <param name="IdAMover"></param>
        /// <param name="IdDestino"></param>
        /// <returns></returns>
        public async Task<bool> EsCarpetaHija(string IdAMover, string IdDestino)
        {
            var c = await this.repo.UnicoAsync(x => x.Id == IdDestino);

            if (c != null)
            {
                if (c.CarpetaPadreId == IdAMover)
                {
                    return true;

                }
                else
                {
                    if (c.CarpetaPadreId != null)
                    {
                        return await EsCarpetaHija(IdAMover, c.Id);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

        }


        public async Task<bool> EsCarpetaEliminada(string IdDestino)
        {
            var c = await this.repo.UnicoAsync(x => x.Id == IdDestino);

            if (c != null)
            {
                if (c.Eliminada)
                {
                    return true;

                }
                else
                {
                    if (c.CarpetaPadreId != null)
                    {
                        return await EsCarpetaEliminada(c.CarpetaPadreId);

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

        }
    }
}
