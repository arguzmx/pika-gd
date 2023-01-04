using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioElemento : IServicioRepositorioAsync<Elemento, string>, IServicioAutenticado<Elemento>
    {
        Task EventoActualizarVersionElemento(int EventoID, string ElementoId, bool Exitoso = true, string ElementoNombre = null, string Delta = null);
        Task<bool> AccesoValidoElemento(string ElementoId, bool Escritura = false);
        Task<List<Elemento>> ObtenerPaginadoByIdsAsync(ConsultaAPI q);
        Task<List<string>> Purgar();
        Task<int> ACLPuntoMontaje(string PuntoMontajeId);
        Task ActualizaVersion(string Id, string VersionId);
        Task ActualizaConteoPartes(string Id, int Conteo);
        Task ActualizaTamanoBytes(string Id, long Tamano);
    }
}
