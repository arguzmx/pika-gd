using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioElemento : IServicioRepositorioAsync<Elemento, string>
    {
        public UsuarioAPI Usuario { get; set; }
        Task<List<Elemento>> ObtenerPaginadoByIdsAsync(ConsultaAPI q);
        Task<List<string>> Purgar();
        Task<int> ACLPuntoMontaje(string PuntoMontajeId);
        Task ActualizaVersion(string Id, string VersionId);
    }
}
