using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Filters
{
    public class PropiedadesFiltroACL
    {

        public string AppId { get; set; }

        public string DomainId { get; set; }

        public string  ModeulId { get; set; }

        //public string  Method { get; set; }
        //public string UserId { get; set; }

    }

    public interface ILocalizadorFiltroACL
    {
        Task<PropiedadesFiltroACL> ObtienePropiedades(Type TipoControlador);

    }


    public class LocalizadorFiltroACLReflectivo : ILocalizadorFiltroACL
    {
        public Task<PropiedadesFiltroACL> ObtienePropiedades(Type TipoControlador)
        {
            throw new NotImplementedException();
        }
    }

}
