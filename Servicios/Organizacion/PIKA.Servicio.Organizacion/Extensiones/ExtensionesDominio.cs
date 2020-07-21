using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion
{
    public static partial class Extensiones
    {
        public static List<ValorListaOrdenada> ValoresLista(this List<Dominio> lista)
        {

            return lista.Select(x => new ValorListaOrdenada() { Id = x.Id, Indice = 0, Texto = x.Nombre }).ToList();

        }
        public static Dominio Copia(this Dominio d)
        {
            if (d == null) return null;
            Dominio data = new Dominio()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };


            if (d.UnidadesOrganizacionales != null)
            {
                foreach (UnidadOrganizacional uo in d.UnidadesOrganizacionales)
                {
                    data.UnidadesOrganizacionales.Add(uo.Copia());
                }

            }

            return data;
        }
    }
}
