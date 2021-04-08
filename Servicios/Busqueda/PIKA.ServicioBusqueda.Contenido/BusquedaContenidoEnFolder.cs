using MySql.Data.MySqlClient;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.ServicioBusqueda.Contenido
{
    public partial class ServicioBusquedaContenido
    {

        private Busqueda BuscarEnFolder(BusquedaContenido busqueda)
        {
            return busqueda.Elementos.Where(x => x.Tag == Constantes.ENFOLDER).FirstOrDefault();
        }

        

        private async Task<long> ContarEnFolder(Consulta q, bool IncluirIds)
        {
            if (IncluirIds)
            {
                IdsEnfolder = new List<string>();
            }
            
            int conteo = 0;
            var Id = q.Filtros.Where(x => x.Propiedad.ToLower() == "id").FirstOrDefault();
            var PuntoMontajeId = q.Filtros.Where(x => x.Propiedad.ToLower() == "puntomontajeid").FirstOrDefault();
            if (Id != null && PuntoMontajeId !=null )
            {
                var connection = new MySqlConnection(this.Configuration["ConnectionStrings:pika-gd"]);
                await connection.OpenAsync();
                conteo = await ConteoRecursivoEnFolder(Id.Valor, PuntoMontajeId.Valor, connection, IncluirIds);
                await connection.CloseAsync();
            }
            return conteo;
        }

        private async Task<int> ConteoRecursivoEnFolder(string Id, string PuntoMontajeId, MySqlConnection cn, bool IncluirIds)
        {

            string sqls = $"SELECT count(*) FROM contenido$carpeta where PuntoMontajeId='{PuntoMontajeId}' and CarpetaPadreId='{Id}'";
            int conteo = 0;
            MySqlCommand cmd = new MySqlCommand(sqls, cn);
            DbDataReader dr = await cmd.ExecuteReaderAsync();
            if (dr.Read())
            {
                conteo = dr.GetInt32(0);
            }
            dr.Close();


            if (conteo > 0)
            {
               List<string> ids = new List<string>(); 
               sqls = $"SELECT Id FROM contenido$carpeta where PuntoMontajeId='{PuntoMontajeId}' and CarpetaPadreId='{Id}'";
               cmd.CommandText = sqls;
                dr = await cmd.ExecuteReaderAsync();
                while(dr.Read())
                {
                    ids.Add(dr.GetString(0));
                }
                dr.Close();

                if (IncluirIds)
                {
                    IdsEnfolder.AddRange(ids);
                }

                foreach (string id in ids)
                {
                    conteo += await ConteoRecursivoEnFolder(id, PuntoMontajeId, cn, IncluirIds);
                }

            }

            cmd.Dispose();
            return conteo;
        }


       

    }
}
