﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Seguridad;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioUsuarios : IServicioRepositorioAsync<PropiedadesUsuario, string>,
          IServicioValorTextoAsync<PropiedadesUsuario>, IServicioAutenticado<PropiedadesUsuario>
    {

        Task RegistroLogin(string Usaurio, bool Valido, string DireccionRed);
        Task<ICollection<string>> Inactivar(string[] ids);
        Task<ICollection<string>> Activar(string[] ids);

        Task<Boolean> EsAdmin(string dominioId, string UnidadOrgId, string Id);

        Task<PropiedadesUsuario> CrearAsync(string dominioId, string UnidadOrgId, PropiedadesUsuario entity, CancellationToken cancellationToken = default);
        Task<int> ActutalizarContrasena(string UsuarioId, string Actual, string nueva);
        Task<int>  ActualizarContrasena(string UsuarioId, string nueva);

        Task<IPaginado<PropiedadesUsuario>> ObtenerPaginadoIdsAsync(List<string> ids, Consulta Query,
            Func<IQueryable<PropiedadesUsuario>,
                IIncludableQueryable<PropiedadesUsuario, object>> include = null,
            bool disableTracking = true, CancellationToken cancellationToken = default);
    }
}
