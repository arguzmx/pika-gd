using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioEntidades.Interfaces
{
    public interface IServicioIdsVinculados
    {

        public Task EliminarVinculosTodos(string Id);
        public Task AdiconarVinculosLista(string Id, List<string> IdsVinculados);

    }
}
