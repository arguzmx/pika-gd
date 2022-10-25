using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class CatalogoVinculado
    {
        public string EntidadCatalogo { get; set; }
        public string IdCatalogo { get; set; }
        public string IdEntidad { get; set; }

        public string IdCatalogoMap { get; set; }
        public string IdEntidadMap { get; set; }
        public string EntidadVinculo { get; set; }
        public string PropiedadReceptora { get; set; }
        public TipoDespliegueVinculo Despliegue { get; set; }
    }

    public static partial class Extensiones
    {
        public static CatalogoVinculado Copia(this LinkCatalogoAttribute link)
        {
            return new CatalogoVinculado()
            {
                Despliegue = link.Despliegue,
                EntidadCatalogo = link.EntidadCatalogo,
                IdCatalogo = link.IdCatalogo,
                IdEntidad = link.IdEntidad,
                EntidadVinculo = link.EntidadVinculo,
                IdEntidadMap = link.IdEntidadMap,
                IdCatalogoMap = link.IdCatalogoMap,
                PropiedadReceptora = link.PropiedadReceptora
            };
        }

        public static LinkVista Copia(this LinkViewAttribute link)
        {
            return new LinkVista()
            {
                Icono = link.Icono,
                Titulo = link.Titulo,
                Vista = link.Vista,
                RequiereSeleccion = link.RequireSeleccion,
                Tipo = link.Tipo,
                Condicion = link.Condicion,
                MenuId = link.MenuId,
                MenuIndex = link.MenuIndex
            };
        }

        public static Menu Copia(this MenuAttribute menu)
        {
            return new Menu()
            {
                Condicion = menu.Condicion,
                Icono = menu.Icono,
                MenuId = menu.MenuId,
                MenuIndex = menu.MenuIndex,
                Titulo = menu.Titulo
            };
        }
        public static ParametroLinkVista Copia(this LinkViewParameterAttribute link)
        {
            return new ParametroLinkVista()
            {
                Vista = link.Vista,
                ParamName = link.ParamName,
                Multiple = link.Multiple
            };
        }
    }
}
