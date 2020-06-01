using Microsoft.AspNetCore.Mvc.ModelBinding;
using RepositorioEntidades;

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
                result.ord_direccion = bindingContext.ActionContext.HttpContext.Request.Query[$"ordd"];
                result.recalcular_totales = bindingContext.ActionContext.HttpContext.Request.Query["recalc"] == "true" ? true : false;

                int columnindex = 0;
                List<string> keys = bindingContext.ActionContext.HttpContext.Request.Query.Keys.ToList();
                bool found = true;
                while (found)
                {
                    if (keys.IndexOf($"f[{columnindex}][p]") >= 0)
                    {
                        string negacion = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][n]"];
                        string nivelFuzzy = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][z]"];
                        bool neg = false;
                        int fuzzy = -1;

                        if (!string.IsNullOrEmpty(negacion))
                        {
                            if (",true,t,1,".IndexOf(negacion, StringComparison.InvariantCultureIgnoreCase) > 0)
                            {
                                neg = true;
                            }
                        }

                        if (!string.IsNullOrEmpty(nivelFuzzy))
                        {
                            if(int.TryParse(nivelFuzzy, out fuzzy))
                            {
                                if (fuzzy > 5) fuzzy = 5;
                                if (fuzzy < -1) fuzzy = -1;
                            }
                        }

                        result.Filtros.Add(new FiltroConsulta()
                        {
                            Propiedad = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][p]"],
                            Operador = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][o]"],
                            Valor = bindingContext.ActionContext.HttpContext.Request.Query[$"f[{columnindex}][v]"],
                            Negacion = neg, 
                            NivelFuzzy = fuzzy
                        });

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
