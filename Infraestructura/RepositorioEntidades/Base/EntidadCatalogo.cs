using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class EntidadCatalogo<T,U>: IEntidadCatalogo<U>
    {
        public  T Id { get; set; }
        public string Nombre { get; set; }

        public virtual List<U> Seed()
        {
            return null;
        }
    }
}
