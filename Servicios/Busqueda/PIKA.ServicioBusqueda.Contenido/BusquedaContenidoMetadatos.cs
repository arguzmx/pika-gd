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
   

        private Busqueda BuscarMetadatos(BusquedaContenido busqueda)
        {
            return busqueda.Elementos.Where(x => x.Tag == Constantes.METADATOS).FirstOrDefault();
        }

           

        private async Task<int> ContarEnMetadatos(Consulta q)
        {
            int conteo = 0;

            return conteo;
        }

    }
}
