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
    public partial class DatatablesModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            var result = new SolicitudDatatables();
            try
            {
                result.draw = int.Parse(bindingContext.ActionContext.HttpContext.Request.Query[$"draw"], CultureInfo.InvariantCulture);
                result.start = int.Parse(bindingContext.ActionContext.HttpContext.Request.Query[$"start"], CultureInfo.InvariantCulture);
                result.length = int.Parse(bindingContext.ActionContext.HttpContext.Request.Query[$"length"], CultureInfo.InvariantCulture);
                result.search.value = bindingContext.ActionContext.HttpContext.Request.Query[$"search[value]"];
                result.search.regex = bindingContext.ActionContext.HttpContext.Request.Query[$"search[regex]"] == "true" ? true : false;
                result.start = (result.start / result.length);
                if (result.start < 0) result.start = 0;

                int columnindex = 0;
                List<string> keys = bindingContext.ActionContext.HttpContext.Request.Query.Keys.ToList();
                bool found = true;
                while (found)
                {
                    if (keys.IndexOf($"columns[{columnindex}][data]") >= 0)
                    {
                        result.columns.Add(new Datatablescolumn()
                        {
                            data = bindingContext.ActionContext.HttpContext.Request.Query[$"columns[{columnindex}][data]"],
                            name = bindingContext.ActionContext.HttpContext.Request.Query[$"columns[{columnindex}][name]"],
                            orderable = bindingContext.ActionContext.HttpContext.Request.Query[$"columns[{columnindex}][orderable]"] == "true" ? true : false,
                            searchable = bindingContext.ActionContext.HttpContext.Request.Query[$"columns[{columnindex}][searchable]"] == "true" ? true : false,
                            search = new DatatablesSearch()
                            {
                                regex = bindingContext.ActionContext.HttpContext.Request.Query[$"columns[{columnindex}][regex][value]"] == "true" ? true : false,
                                value = bindingContext.ActionContext.HttpContext.Request.Query[$"columns[{columnindex}][search][value]"]
                            }
                        });
                        columnindex++;
                    }
                    else
                    {
                        found = false;
                    }
                }

                columnindex = 0;
                found = true;
                while (found)
                {
                    if (keys.IndexOf($"order[{columnindex}][column]") >= 0)
                    {

                        result.order.Add(new DatatatablesOrder()
                        {
                            column = int.Parse(bindingContext.ActionContext.HttpContext.Request.Query[$"order[{columnindex}][column]"], CultureInfo.InvariantCulture),
                            dir = bindingContext.ActionContext.HttpContext.Request.Query[$"order[{columnindex}][dir]"]
                        });

                        columnindex++;
                    }
                    else
                    {
                        found = false;
                    }
                }


                columnindex = 0;
                found = true;
                while (found)
                {
                    if (keys.IndexOf($"f[{columnindex}][p]") >= 0)
                    {
                        bool filterok = true;
                        FiltroConsulta  f = new FiltroConsulta();
                        f.Propiedad = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][p]"];
                        if (keys.IndexOf($"f[{columnindex}][o]") < 0)
                        {
                            filterok = false;
                        }
                        if (keys.IndexOf($"f[{columnindex}][v]") < 0)
                        {
                            filterok = false;
                        }

                        if (filterok)
                        {
                            f.Operador = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][o]"];
                            f.Valor = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][v]"];

                            result.Filters.Add(f);
                        }


                        columnindex++;
                    }
                    else
                    {
                        found = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }


            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}
