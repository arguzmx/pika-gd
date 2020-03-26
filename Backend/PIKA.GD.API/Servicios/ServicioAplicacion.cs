using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API
{
    public static class ServicioAplicacion
    {

        private static List<MetadataInfo> _InformacionMetadatos;

        public static List<TipoAdministradorModulo> ModulosAdministrados { get; set; }

        private static object LockInfoMetadatos = new object();

        public static MetadataInfo InformacionMetadatos(Type Tipo)
        {
            if (_InformacionMetadatos == null) return null;
            return _InformacionMetadatos.Where(x => x.Tipo == Tipo.Name).Take(1).SingleOrDefault(); ;
        }


        public static void AdiconaInformacionMetadatos(MetadataInfo info)
        {
            lock (LockInfoMetadatos)
            {
                if (_InformacionMetadatos == null) _InformacionMetadatos = new List<MetadataInfo>();
                _InformacionMetadatos.Add(info);
            }
        }


    }
}
