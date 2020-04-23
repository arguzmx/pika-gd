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
            return new Plantilla()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };
        }
        public static PropiedadPlantilla CopiaPropiedadPlantilla(this PropiedadPlantilla d)
        {
            return new PropiedadPlantilla()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Autogenerado = d.Autogenerado,
                Buscable = d.Buscable,
                ControlHTML = d.ControlHTML,
                ValorDefault = d.ValorDefault,
                IndiceOrdenamiento = d.IndiceOrdenamiento

            };
        }
        public static TipoDato CopiaTipoDato(this TipoDato d)
        {
            return new TipoDato()
            {
                Id = d.Id,
                Nombre = d.Nombre
            };
        }
        public static TipoDatoPropiedadPlantilla CopiaTipoDatoPropiedadPlantilla(this TipoDatoPropiedadPlantilla d)
        {
            return new TipoDatoPropiedadPlantilla()
            {
                PropiedadPlantillaId = d.PropiedadPlantillaId,
                TipoDatoId = d.TipoDatoId

            };
        }
        public static AtributoMetadato CopiaAtributoMetadato(this AtributoMetadato d)
        {
            return new AtributoMetadato()
            {
                Id = d.Id,
                PropiedadId=d.PropiedadId,
                Valor=d.Valor
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
                valordefaulr=d.valordefaulr
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
                TipoAlmacenMetadatosId=d.TipoAlmacenMetadatosId,
                TipoOrigenId=d.TipoOrigenId
            };
        }
        public static TipoAlmacenMetadatos CopiaTipoAlmacenMetadatos(this TipoAlmacenMetadatos d)
        {
            return new TipoAlmacenMetadatos()
            {
                Id = d.Id,
                AsociacionPlantillaid=d.AsociacionPlantillaid,
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