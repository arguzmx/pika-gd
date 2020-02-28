using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class DuplaCatalogo<T, U>
    {
        public T Id { get; set; }
        public U Valor { get; set; }
    }
}
