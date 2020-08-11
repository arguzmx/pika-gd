using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Ampliacion Copia(this Ampliacion a)
        {
            if (a == null) return null;
            return new Ampliacion()
            {
                Id=a.Id,
                ActivoId = a.ActivoId,
                Vigente = a.Vigente,
                TipoAmpliacionId = a.TipoAmpliacionId,
                FechaFija = a.FechaFija,
                FundamentoLegal = a.FundamentoLegal,
                Inicio = a.Inicio,
                Fin = a.Fin,
                Anos = a.Anos,
                Meses = a.Meses,
                Dias = a.Dias

            };
        }
    }
}
