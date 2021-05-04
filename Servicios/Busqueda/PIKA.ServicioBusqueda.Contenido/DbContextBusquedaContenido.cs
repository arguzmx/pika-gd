using Microsoft.EntityFrameworkCore;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.ServicioBusqueda.Contenido
{
    public class DbContextContenidoFactory : IFabricaContexto<DbContextBusquedaContenido>
    {

        private IProveedorOpcionesContexto<DbContextBusquedaContenido> proveedorOpciones;
        public DbContextContenidoFactory(IProveedorOpcionesContexto<DbContextBusquedaContenido> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextBusquedaContenido Crear()
        {
            return new DbContextBusquedaContenido(proveedorOpciones.ObtieneOpciones());
        }
    }

    public class DbContextBusquedaContenido : DbContext
    {

        public DbContextBusquedaContenido(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<ElementoBusqueda> Elementos { get; set; }

    }
}
