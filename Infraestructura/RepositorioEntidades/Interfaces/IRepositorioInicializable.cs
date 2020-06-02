using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public interface IRepositorioInicializable
    {
        void Inicializar(string contentPath, bool generarDatosdemo);
        void AplicarMigraciones();

    }


 

}
