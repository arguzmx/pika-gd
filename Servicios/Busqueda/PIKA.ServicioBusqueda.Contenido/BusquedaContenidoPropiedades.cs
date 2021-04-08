using MySql.Data.MySqlClient;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        

        private async Task<long> ContarPropiedades(Consulta q, bool IncluirIds)
        {
            int conteo = 0;
            List<string> condiciones = MySQLQueryComposer.Condiciones<Elemento>(q);
            StringBuilder query = new StringBuilder();
            var PuntoMontajeId = q.Filtros.Where(x => x.Propiedad.ToLower() == "puntomontajeid").FirstOrDefault();
            string contexto = "count(*)";
            

            var cn = new MySqlConnection(this.Configuration["ConnectionStrings:pika-gd"]);
            await cn.OpenAsync();

            string sqls = $"select {contexto}  from contenido$elemento where PuntoMontajeId='{PuntoMontajeId.Valor}' ";
            foreach(string s in condiciones)
            {
                sqls += $" and ({s})";
            }

            MySqlCommand cmd = new MySqlCommand(sqls, cn);
            DbDataReader dr = await cmd.ExecuteReaderAsync();
            if (dr.Read())
            {
                conteo = dr.GetInt32(0);
            }
            dr.Close();

            if (IncluirIds)
            {
                contexto = "Id";
                sqls = $"select {contexto}  from contenido$elemento where PuntoMontajeId='{PuntoMontajeId.Valor}' ";
                cmd.CommandText = sqls;
                dr = await cmd.ExecuteReaderAsync();
                IdsPropiedades = dr.Select<string>(r => r.GetString(0) ).ToList();
                dr.Close();
            }

            await dr.DisposeAsync();
            await cn.CloseAsync();

            return conteo;
        }
    }
}
