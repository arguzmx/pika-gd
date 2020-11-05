using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PIKA.Modelo.Metadatos.Extractores
{

    public static class ExtenderPnatilla
    {

        public static Propiedad ToPropiedad(this PropiedadPlantilla pp)
        {
            Propiedad p = new Propiedad()
            {
                AlternarEnTabla = pp.AlternarEnTabla,
                Autogenerado = pp.Autogenerado,
                Buscable = pp.Buscable,
                Contextual = pp.Contextual,
                ControlHTML = pp.ControlHTML,
                EsFiltroJerarquia = pp.EsFiltroJerarquia,
                EsIdClaveExterna = pp.EsIdClaveExterna,
                EsIdJerarquia = pp.EsIdJerarquia,
                EsIdRaizJerarquia = pp.EsIdRaizJerarquia,
                EsIdRegistro = pp.EsIdRegistro,
                EsIndice = pp.EsIndice,
                EsTextoJerarquia = pp.EsTextoJerarquia,
                Etiqueta = pp.Etiqueta,
                Id = pp.Id,
                IdContextual = pp.IdContextual,
                IndiceOrdenamiento = pp.IndiceOrdenamiento,
                IndiceOrdenamientoTabla = pp.IndiceOrdenamientoTabla,
                MostrarEnTabla = pp.MostrarEnTabla,
                Nombre = pp.Nombre,
                Ordenable = pp.Ordenable,
                Requerido = pp.Requerido,
                TipoDatoId = pp.TipoDatoId,
                Visible = pp.Visible,
                ValorDefault = pp.ValorDefault, OrdenarValoresListaPorNombre = pp.OrdenarValoresListaPorNombre,
                ValidadorTexto = pp.ValidadorTexto, ValidadorNumero = pp.ValidadorNumero
            };

            return p;

        }

    }


    public class PlantillaMetadataExtractor
    {

        public MetadataInfo Obtener(Plantilla p)
        {

            MetadataInfo m = new MetadataInfo()
            {
                Tipo = p.Id,
                FullName = p.Nombre,
                EntidadesVinculadas = new List<EntidadVinculada>(),
                CatalogosVinculados = new List<CatalogoVinculado>()
            };

            foreach( var prop in p.Propiedades)
            {
                m.Propiedades.Add(prop.ToPropiedad());
            }

            return m;

        }
    }
}
