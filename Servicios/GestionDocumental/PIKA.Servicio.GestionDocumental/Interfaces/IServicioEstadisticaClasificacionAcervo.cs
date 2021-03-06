﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
   public interface IServicioEstadisticaClasificacionAcervo 
    {
        Task<byte[]> ReporteEstadisticaArchivoCuadro(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros);
        Task<bool> ActualizaConteoEstadistica(string CuadroClasificacionId, string EntradaClasificacionId,
            string ArchivoId, int cantidadActivos, DateTime? fechaMinima, DateTime? fechaMaxima);
    }
}
