using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
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


        public async Task<HashSet<ElementoBusqueda>> BuscarPorIds(BusquedaContenido busqueda, List<string> Ids, bool DeMetadatos)
        {

            try
            {

                this.UDT.Context.Database.GetDbConnection().Open();
                var cmd = this.UDT.Context.Database.GetDbConnection().CreateCommand();

                string tableName = $"temp{ busqueda.Id.Replace("-", "")}";

                string tempT = $@"CREATE TEMPORARY TABLE {tableName} (Id varchar(128) PRIMARY KEY )";
                string dropT = $"DROP TEMPORARY TABLE {tableName}";
                string bulk = $@"insert into `{tableName}`(`Id`) values";

                cmd.CommandText = tempT;
                var a = await cmd.ExecuteScalarAsync();

                int pageIndex = 0;
                StringBuilder insert = new StringBuilder();
                for (int i = 0; i < Ids.Count; i++)
                {
                    if (pageIndex == 0)
                    {
                        insert.Append(bulk + $"('{Ids[i]}'),");
                    }
                    else
                    {
                        insert.Append($"('{Ids[i]}'),");
                    }
                    pageIndex++;

                    if (pageIndex > 100)
                    {
                        cmd.CommandText = insert.ToString().TrimEnd(',');
                        await cmd.ExecuteNonQueryAsync();
                        insert.Clear();
                        pageIndex = 0;
                    }
                }


                if (insert.ToString().EndsWith(','))
                {
                    cmd.CommandText = insert.ToString().TrimEnd(',');
                    await cmd.ExecuteNonQueryAsync();
                    insert.Clear();
                }

                string sqls;
                if (DeMetadatos)
                {
                    sqls = $"select distinct c.*  from contenido$elemento  c inner join {tableName}  t on c.Id = t.Id order by t.Id";

                } else
                {
                    long offset = busqueda.indice == 0 ? 0 : ((busqueda.indice) * busqueda.tamano) - 1;
                    var filtros = busqueda.ObtenerBusqueda(Constantes.PROPIEDEDES);

                    sqls = $"select distinct c.*  from contenido$elemento  c inner join {tableName}  t on c.Id = t.Id ";
                    sqls += $" where c.PuntoMontajeId='{busqueda.PuntoMontajeId}' ";
                    
                    if (filtros != null)
                    {
                        List<string> condiciones = MySQLQueryComposer.Condiciones<ElementoBusqueda>(filtros.Consulta, "c.");
                        foreach (string s in condiciones)
                        {
                            sqls += $" and ({s})";
                        }
                    }
                    sqls += $" order by {busqueda.ord_columna} {busqueda.ord_direccion} ";
                    sqls += $" LIMIT {offset},{busqueda.tamano}";

                }
             
                
                HashSet<ElementoBusqueda> Lista = this.UDT.Context.Elementos.FromSqlRaw(sqls).ToHashSet();

                return Lista;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<long> ContarPropiedades(Consulta q, bool IncluirIds, string PuntoMontajeId)
        {
            try
            {



                int conteo = 0;
                List<string> condiciones = MySQLQueryComposer.Condiciones<ElementoBusqueda>(q);
                StringBuilder query = new StringBuilder();
                string contexto = "count(*)";


                var cn = new MySqlConnection(this.Configuration["ConnectionStrings:pika-gd"]);
                await cn.OpenAsync();

                string sqls = $"select {contexto}  from contenido$elemento where PuntoMontajeId='{PuntoMontajeId}' ";
                foreach (string s in condiciones)
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
                    sqls = $"select {contexto}  from contenido$elemento where PuntoMontajeId='{PuntoMontajeId}' ";
                    foreach (string s in condiciones)
                    {
                        sqls += $" and ({s})";
                    }

                    cmd.CommandText = sqls;
                    dr = await cmd.ExecuteReaderAsync();
                    IdsPropiedades = dr.Select<string>(r => r.GetString(0)).ToList();
                    dr.Close();
                }

                await dr.DisposeAsync();
                await cn.CloseAsync();

                return conteo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
