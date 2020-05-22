using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Modelo.Metadatos
{
    public interface IProveedorMetadatos<T>
    {
        Task<MetadataInfo> Obtener();

    }
}
