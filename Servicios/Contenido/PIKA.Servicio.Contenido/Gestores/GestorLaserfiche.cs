using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Gestores
{
    public class GestorLaserfiche : GestorBase, IGestorES
    {
        public bool ConexionValida()
        {
            throw new NotImplementedException();
        }

        public Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido, FileInfo informacion, bool sobreescribir)
        {
            throw new NotImplementedException();
        }

        public Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, string archivoFuente, FileInfo informacion, bool sobreescribir)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> LeeBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> LeeThumbnailBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            throw new NotImplementedException();
        }

        public Task<string> ObtienePDF(Modelo.Contenido.Version version, List<string> parteIds)
        {
            throw new NotImplementedException();
        }

        public Task<string> ObtieneZIP(Modelo.Contenido.Version version, List<string> parteIds)
        {
            throw new NotImplementedException();
        }
    }
}
