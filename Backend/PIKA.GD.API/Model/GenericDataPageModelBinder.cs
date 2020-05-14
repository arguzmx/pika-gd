using Microsoft.AspNetCore.Mvc.ModelBinding;
using RepositorioEntidades;
using RepositorioEntidades.DatatablesPlugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace PIKA.GD.API.Model
{
    public class GenericDataPageModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            var result = new Consulta();
            try
            {
                result.indice = int.Parse(bindingContext.ActionContext.HttpContext.Request.Query[$"i"], CultureInfo.InvariantCulture);
                result.tamano = int.Parse(bindingContext.ActionContext.HttpContext.Request.Query[$"t"], CultureInfo.InvariantCulture);
                result.ord_columna = bindingContext.ActionContext.HttpContext.Request.Query[$"ordc"];
                result.ord_direccion= bindingContext.ActionContext.HttpContext.Request.Query[$"ordd"];
                result.recalcular_totales = bindingContext.ActionContext.HttpContext.Request.Query["recalc"] == "true"?  true: false ; 

                int columnindex = 0;
                List<string> keys = bindingContext.ActionContext.HttpContext.Request.Query.Keys.ToList();
                bool found = true;
                while (found)
                {
                    if (keys.IndexOf($"f[{columnindex}][p]") >= 0)
                    {

                        result.Filtros.Add(new FiltroConsulta()
                        {
                            Propiedad = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][p]"], 
                            Operador= bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][o]"],
                             Valor = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][v]"]
                        }); ;

                        columnindex++;
                    }
                    else
                    {
                        found = false;
                    }
                }


            }
            catch (Exception)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }


            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}
