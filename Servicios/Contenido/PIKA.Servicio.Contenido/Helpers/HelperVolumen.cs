using Microsoft.EntityFrameworkCore;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Helpers
{
    public class  HelperVolumen
    {
        private IRepositorioAsync<Volumen> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        public HelperVolumen( UnidadDeTrabajo<DbContextContenido> UDT)
        {
            this.UDT = UDT;
            this.repo = this.UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
        }


        public async Task<long> ActualizaTamanoVolumen(string volumenId, long bytesantes, long bytesdespues)
        {
            var v = await this.repo.UnicoAsync(x => x.Id == volumenId);

  
        
            v.Tamano = (v.Tamano - bytesantes) + bytesdespues;

            UDT.Context.Entry(v).State = EntityState.Modified;

            return v.ConsecutivoVolumen;
        }

        public async Task<long> GetConsecutivoVolumen(string volumenId, long bytes)
        {

            var v  = await this.repo.UnicoAsync(x => x.Id == volumenId);

            if((!v.EscrituraHabilitada) || v.Eliminada || (!v.ConfiguracionValida) || (!v.Activo) )
            {
                throw new ExDatosNoValidos($"El volumen {volumenId} no esta disponible");
            }

            if (v.TamanoMaximo > 0)
            {
                if ((v.Tamano + bytes) > v.TamanoMaximo)
                {
                    throw new ExDatosNoValidos($"Tamaño maximo excedido volumen {volumenId}");
                }

            }


            v.CanidadElementos += 1;
            v.ConsecutivoVolumen += 1;
            v.Tamano += bytes;

            UDT.Context.Entry(v).State = EntityState.Modified;

            return v.ConsecutivoVolumen;
        }

      
        
        
    }
}
