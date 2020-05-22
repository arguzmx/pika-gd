using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos.Servicios
{
    public static class ExtensionesMetadatos
    {
        public static Plantilla CopiaPlantilla(this Plantilla d)
        {
            Plantilla p = new Plantilla()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };

            foreach(PropiedadPlantilla pp in d.Propiedades)
            {
                p.Propiedades.Add(pp.CopiaPropiedadPlantilla());

            }

            return p;
        }
        public static PropiedadPlantilla CopiaPropiedadPlantilla(this PropiedadPlantilla d)
        {
            PropiedadPlantilla pp= new PropiedadPlantilla()
            {
                PlantillaId = d.PlantillaId,
                EsIndice = d.EsIndice,
                EsFiltroJerarquia = d.EsFiltroJerarquia,
                EsIdClaveExterna = d.EsIdClaveExterna,
                EsIdJerarquia = d.EsIdJerarquia,
                EsIdPadreJerarquia = d.EsIdPadreJerarquia,
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

            if (d.TipoDato != null) {
                pp.TipoDato = d.TipoDato.CopiaTipoDato();
            }

            if (d.ValidadorNumero != null)
            {
                pp.ValidadorNumero = d.ValidadorNumero.CopiaValidadorNumero();
            }

            if (d.ValidadorTexto != null)
            {
                pp.ValidadorTexto = d.ValidadorTexto.CopiaValidadorTexto();
            }


            if (d.AtributoTabla != null)
            {
                pp.AtributoTabla = d.AtributoTabla.CopiaAtributoTabla();
            }

            return pp;
        }
        public static TipoDato CopiaTipoDato(this TipoDato d)
        {
            return new TipoDato()
            {
                Id = d.Id,
                Nombre = d.Nombre
            };
        }

    
        public static ValidadorNumero CopiaValidadorNumero(this ValidadorNumero d)
        {
            return new ValidadorNumero()
            {
                Id = d.Id,
                PropiedadId = d.PropiedadId,
                valordefault=d.valordefault,
                max=d.max,
                min=d.min
            };
        }
        public static ValidadorTexto CopiaValidadorTexto(this ValidadorTexto d)
        {
            return new ValidadorTexto()
            {
                Id = d.Id,
                PropiedadId = d.PropiedadId,
                longmax=d.longmax,
                longmin=d.longmin,
                regexp=d.regexp,
                valordefault=d.valordefault
            };
        }
        public static AsociacionPlantilla CopiaAsociacionPlantilla(this AsociacionPlantilla d)
        {
            return new AsociacionPlantilla()
            {
                Id = d.Id,
                IdentificadorAlmacenamiento=d.IdentificadorAlmacenamiento,
                OrigenId=d.OrigenId,
                PlantillaId=d.PlantillaId,
                TipoOrigenId=d.TipoOrigenId
            };
        }
        public static TipoAlmacenMetadatos CopiaTipoAlmacenMetadatos(this TipoAlmacenMetadatos d)
        {
            return new TipoAlmacenMetadatos()
            {
                Id = d.Id,
                Nombre=d.Nombre
            };
        }

     
        public static AtributoTabla CopiaAtributoTabla(this AtributoTabla d)
        {
            return new AtributoTabla()
            {
                Id = d.Id,
                PropiedadId = d.PropiedadId,
                Alternable=d.Alternable,
                IdTablaCliente=d.IdTablaCliente,
                Incluir=d.Incluir,
                IndiceOrdebnamiento=d.IndiceOrdebnamiento,
                Visible=d.Visible

            };
        }
    }
}