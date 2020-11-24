using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioEntidades
{
    public interface IRepositorioInicializableAutoConfigurable
    {
        Task Inicializar(IConfiguration configuration, string contentPath, bool generarDatosdemo, ILoggerFactory loggerFactory);

    }
}
