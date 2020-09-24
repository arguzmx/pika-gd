using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IComunesPartes
    {
        Task<List<Parte>> CrearAsync(List<Parte> partes);
        Task<List<Parte>> ObtienePartesVersion(string ElementoId, string VersionId);
    }
}
