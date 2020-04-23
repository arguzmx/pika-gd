using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Organizacion.Servicios
{
    public static class ExtensionesOrganizacion
    {

        //Un metodo de extension extioene un objeto en este caso Domnio con esta funcion y se le pasa el objeto
        //utilizando thin como se ve
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

        public static UnidadOrganizacional CopiaUO(this UnidadOrganizacional d)
        {
            return new UnidadOrganizacional()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Eliminada = d.Eliminada
            };
        }

        public static DireccionPostal CopiaDireccionPostal(this DireccionPostal d)
        {
            return new DireccionPostal()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };
        }

        public static Rol CopiaRol(this Rol d)
        {
            return new Rol()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };
        }
    }
}
