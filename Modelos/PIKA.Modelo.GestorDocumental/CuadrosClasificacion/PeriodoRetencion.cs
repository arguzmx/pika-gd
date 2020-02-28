using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    /// <summary>
    /// Determina los periodo de retecnicón para los elementos de clasiificaicon
    /// </summary>
    public class PeriodoRetencion : EntidadCatalogo<string, PeriodoRetencion>
    {
        public override List<PeriodoRetencion> Seed()
        {
            List<PeriodoRetencion> l = new List<PeriodoRetencion>();
            l.Add(new PeriodoRetencion() { Id = ConstantesArchivo.IDRETENCION_ARCHIVO_TRAMITE, Nombre = "Trámite" });
            l.Add(new PeriodoRetencion() { Id = ConstantesArchivo.IDRETENCION_ARCHIVO_CONSERVACION , Nombre = "Conservación" });
            l.Add(new PeriodoRetencion() { Id = ConstantesArchivo.IDRETENCION_ARCHIVO_HISTORICO , Nombre = "Histórico" });
            return l;
        }

    }
}
