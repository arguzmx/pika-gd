using Bogus.DataSets;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.Servicio.Organizacion.Servicios
{
    public static class ExtensionesOrganizacion
    {


        public static List<ValorListaOrdenada> ValoresLista(this List<Dominio>  lista) {

            return lista.Select(x => new ValorListaOrdenada() { Id = x.Id, Indice = 0, Texto = x.Nombre }).ToList();
        
        }


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
