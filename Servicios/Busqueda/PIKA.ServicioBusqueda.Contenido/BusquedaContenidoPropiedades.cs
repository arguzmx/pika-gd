using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.ServicioBusqueda.Contenido
{
    public partial class ServicioBusquedaContenido
    {

        private Busqueda BuscarPropiedades(BusquedaContenido busqueda)
        {
            return busqueda.Elementos.Where(x => x.Tag == Constantes.PROPIEDEDES).FirstOrDefault();
        }

        private async Task<int> ContarPropiedades(Consulta q, bool IncluirIds)
        {
            int conteo = 0;
            List<string> condiciones = MySQLQueryComposer.Condiciones<Elemento>(q);
            StringBuilder query = new StringBuilder();
            
            string contexto = IncluirIds ? "Id" : "count(*)";
            

            return conteo;
        }
    }
}
