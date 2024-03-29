﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioActivo : IServicioRepositorioAsync<Activo, string>, IRepositorioEntidadSeleccionada<string, Activo>,
        IServicioValorTextoAsync<Activo>, IServicioBuscarTexto<Activo>, IServicioAutenticado<Activo>
    {
        Task<byte[]> ImportarActivos(byte[]file,string ArchivId,string TipoId, string OrigenId,string formatoFecha);
        Task<List<string>> Purgar();
        Task<byte[]> ReporteCaratulaActivo(string Dominio, string UnidadOrganizacinal, string ActivoId);

    }
}