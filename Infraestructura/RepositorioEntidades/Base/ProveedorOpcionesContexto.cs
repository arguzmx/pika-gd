using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{

    public interface IProveedorOpcionesContexto<T>
    {
        DbContextOptions ObtieneOpciones();
    }


    public class ProveedorOpcionesContexto<T> : IProveedorOpcionesContexto<T>
    {
        private readonly IConfiguration configuracion;
        public ProveedorOpcionesContexto(IConfiguration configuracion)
        {
            this.configuracion = configuracion;
        }

        public DbContextOptions ObtieneOpciones()
        {
            var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(typeof(T));
            var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType);
            optionsBuilder.UseMySql(configuracion.GetConnectionString("pika-gd"));
            return optionsBuilder.Options;
        }
    }

}
