﻿using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RepositorioEntidades.Interfaces;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioActivoTransferencia : IServicioRepositorioAsync<ActivoTransferencia, string>, IServicioIdsVinculados, 
        IServicioAutenticado<ActivoTransferencia>, IServicioBuscarTexto<ActivoTransferencia>
    {
        Task<RespuestaComandoWeb> ComandoWeb(string command, object payload);

        Task<ICollection<string>> EliminarActivoTransferencia(string[] ids);
    }
}
