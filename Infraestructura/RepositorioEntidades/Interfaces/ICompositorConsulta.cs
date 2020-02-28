using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositorioEntidades
{
    public interface ICompositorConsulta<T>
    {
        Expression Componer(ParameterExpression pe, Consulta q);

    }
}
