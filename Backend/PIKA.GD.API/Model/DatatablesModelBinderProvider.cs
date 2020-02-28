using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RepositorioEntidades.DatatablesPlugin;

namespace PIKA.GD.API.Model
{
    public class DatatablesModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(SolicitudDatatables))
                return new BinderTypeModelBinder(typeof(DatatablesModelBinder));

            return null;
        }
    }
}
