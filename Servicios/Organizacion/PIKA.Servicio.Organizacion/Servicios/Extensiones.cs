using Bogus.DataSets;
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
            Dominio data= new Dominio()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };


            if (d.UnidadesOrganizacionales != null) {
                foreach (UnidadOrganizacional uo in d.UnidadesOrganizacionales) {
                    data.UnidadesOrganizacionales.Add(uo.CopiaUO());
                }

            }

            return data;
        }

        public static UnidadOrganizacional CopiaUO(this UnidadOrganizacional d)
        {
            UnidadOrganizacional data= new UnidadOrganizacional()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Eliminada = d.Eliminada, 
                DominioId = d.DominioId
            };

            return data;
        }

        public static DireccionPostal CopiaDireccionPostal(this DireccionPostal d)
        {
            DireccionPostal dp =
            new DireccionPostal()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                EstadoId = d.EstadoId,
                Municipio = d.Municipio,
                CP = d.CP,
                Calle = d.Calle,
                Colonia = d.Colonia,
                NoExterno = d.NoExterno,
                NoInterno = d.NoInterno,
                PaisId = d.PaisId,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };

            if (d.Estado != null) {
                dp.Estado = d.Estado.CopiaEstado();
            }

            if (d.Pais != null)
            {
                dp.Pais = d.Pais.CopiaPais();
            }

            return dp;
        }

        public static Pais CopiaPais(this Pais d)
        {
            return new Pais()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Estados = null
            };
        }

        public static Estado CopiaEstado(this Estado d)
        {
            return new Estado()
            {
                Id = d.Id,
                Nombre = d.Nombre, 
                PaisId = d.PaisId
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
