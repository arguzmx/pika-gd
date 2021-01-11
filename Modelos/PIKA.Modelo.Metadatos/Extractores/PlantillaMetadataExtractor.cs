using Microsoft.EntityFrameworkCore.Internal;
using PIKA.Modelo.Metadatos.Atributos;
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
                ValidadorTexto = pp.ValidadorTexto, ValidadorNumero = pp.ValidadorNumero,
                AtributosVistaUI = new List<AtributoVistaUI>()
        };


            var uiatt = new AtributoVistaUI()
            {
                Accion = Atributos.Acciones.addupdate,
                Control = "",
                Plataforma = "web",
                PropiedadId = p.Id
            };

            switch (p.TipoDatoId)
            {
                case TipoDato.tBinaryData:
                    uiatt.Control = ControlUI.HTML_FILE;
                    break;

                case TipoDato.tBoolean:
                    uiatt.Control = ControlUI.HTML_TOGGLE;
                    break;

                case TipoDato.tDate:
                    uiatt.Control = ControlUI.HTML_DATE;
                    break;

                case TipoDato.tDateTime:
                    uiatt.Control = ControlUI.HTML_DATETIME;
                    break;

                case TipoDato.tDouble:
                    uiatt.Control = ControlUI.HTML_NUMBER;
                    break;

                case TipoDato.tIndexedString:
                    uiatt.Control = ControlUI.HTML_TEXTAREA;
                    break;

                case TipoDato.tInt32:
                    uiatt.Control = ControlUI.HTML_NUMBER;
                    break;

                case TipoDato.tInt64:
                    uiatt.Control = ControlUI.HTML_NUMBER;
                    break;

                case TipoDato.tList:
                    uiatt.Control = ControlUI.HTML_SELECT;
                    break;

                case TipoDato.tString:
                    uiatt.Control = ControlUI.HTML_TEXT;
                    break;

                case TipoDato.tTime:
                    uiatt.Control = ControlUI.HTML_TIME;
                    break;

            }

            p.AtributosVistaUI.Add(uiatt);



            return p;

        }

    }


    public class PlantillaMetadataExtractor
    {

        public MetadataInfo Obtener(Plantilla p)
        {
            try
            {
                MetadataInfo m = new MetadataInfo()
                {
                    Tipo = p.Id,
                    FullName = p.Nombre,
                    EntidadesVinculadas = new List<EntidadVinculada>(),
                    CatalogosVinculados = new List<CatalogoVinculado>()
                };

                foreach (var prop in p.Propiedades)
                {
                    var pm = prop.ToPropiedad();
                    Console.WriteLine($"{prop.Nombre} {prop.ValoresLista.Count}");
                    if (prop.ValoresLista != null
                        && prop.ValoresLista.Count > 0)
                    {
                        pm.AtributoLista = new AtributoLista();
                        pm.AtributoLista.DatosRemotos = false;
                        foreach (var l in prop.ValoresLista)
                        {
                            pm.AtributoLista.Valores.Add(new ValorLista()
                            {
                                Id = l.Id,
                                Indice = l.Indice,
                                Texto = l.Texto
                            });

                        }
                    }

                    m.Propiedades.Add(pm);
                }

                return m;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }



        }
    }
}
