using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static PropiedadPlantilla Copia(this PropiedadPlantilla d)
        {
            if (d == null) return null;
            PropiedadPlantilla pp = new PropiedadPlantilla()
            {
                PlantillaId = d.PlantillaId,
                EsIndice = d.EsIndice,
                EsFiltroJerarquia = d.EsFiltroJerarquia,
                EsIdClaveExterna = d.EsIdClaveExterna,
                EsIdJerarquia = d.EsIdJerarquia,
                EsIdRaizJerarquia= d.EsIdRaizJerarquia,
                EsIdRegistro = d.EsIdRegistro,
                EsTextoJerarquia = d.EsTextoJerarquia,
                Ordenable = d.Ordenable,
                Requerido = d.Requerido,
                TipoDatoId = d.TipoDatoId,
                Visible = d.Visible,
                Id = d.Id,
                Nombre = d.Nombre,
                Autogenerado = d.Autogenerado,
                Buscable = d.Buscable,
                ControlHTML = d.ControlHTML,
                ValorDefault = d.ValorDefault,
                IndiceOrdenamiento = d.IndiceOrdenamiento

            };

            if (d.TipoDato != null)
            {
                pp.TipoDato = d.TipoDato.Copia();
            }

            if (d.ValidadorNumero != null)
            {
                pp.ValidadorNumero = d.ValidadorNumero.Copia();
            }

            if (d.ValidadorTexto != null)
            {
                pp.ValidadorTexto = d.ValidadorTexto.Copia();
            }


            if (d.AtributoTabla != null)
            {
                pp.AtributoTabla = d.AtributoTabla.Copia();
            }

            return pp;
        }
    }
}
