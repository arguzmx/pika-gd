using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Organizacion.Servicios
{
    public static class ExtensionesOrganizacion
    {
        public static Dominio CopiaDominio(this Dominio d)
        {
            return new Dominio()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };
        }
    }
}
