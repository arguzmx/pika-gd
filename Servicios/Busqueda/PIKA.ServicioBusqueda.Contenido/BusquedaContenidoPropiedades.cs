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


        public async Task<HashSet<Elemento>> BuscarPorIds(BusquedaContenido busqueda, List<string> Ids)
        {

            try
            {
                this.UDT.Context.Database.GetDbConnection().Open();
                var cmd = this.UDT.Context.Database.GetDbConnection().CreateCommand();
                
            //    Console.WriteLine(this.Configuration["ConnectionStrings:pika-gd"]);
            //var cn = new MySqlConnection(this.Configuration["ConnectionStrings:pika-gd"]);
            //await cn.OpenAsync();

            //if (Ids.Count > 100)
            //{

            string tableName = $"temp{ busqueda.Id.Replace("-", "")}";
                Console.WriteLine(tableName);
            string tempT = $@"CREATE TEMPORARY TABLE {tableName} (Id varchar(128) PRIMARY KEY )";
                Console.WriteLine(tempT);
                string dropT = $"DROP TEMPORARY TABLE {tableName}";
                Console.WriteLine(dropT);
                string bulk = $@"insert into `{tableName}`(`Id`) values";



                //MySqlCommand cmd = new MySqlCommand(tempT, cn);
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

                string x = $"select count(*) from {tableName}";
                cmd.CommandText = x;

                var xx = cmd.ExecuteScalar();

                xx.LogS();


                string sqls = $"select distinct c.*  from contenido$elemento  c inner join {tableName}  t on c.Id = t.Id";

                Console.WriteLine(sqls);
                HashSet<Elemento> Lista = this.UDT.Context.Elementos.FromSqlRaw(sqls).ToHashSet();
                Console.WriteLine(">>>");
                Lista.LogS();
            return Lista;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private async Task<long> ContarPropiedades(Consulta q, bool IncluirIds)
        {
            try
            {



                int conteo = 0;
                List<string> condiciones = MySQLQueryComposer.Condiciones<Elemento>(q);
                StringBuilder query = new StringBuilder();
                var PuntoMontajeId = q.Filtros.Where(x => x.Propiedad.ToLower() == "puntomontajeid").FirstOrDefault();
                string contexto = "count(*)";


                var cn = new MySqlConnection(this.Configuration["ConnectionStrings:pika-gd"]);
                await cn.OpenAsync();

                string sqls = $"select {contexto}  from contenido$elemento where PuntoMontajeId='{PuntoMontajeId.Valor}' ";
                foreach (string s in condiciones)
                {
                    sqls += $" and ({s})";
                }

                Console.WriteLine(sqls);
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
                    IdsPropiedades = dr.Select<string>(r => r.GetString(0)).ToList();
                    dr.Close();
                }

                await dr.DisposeAsync();
                await cn.CloseAsync();

                return conteo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
