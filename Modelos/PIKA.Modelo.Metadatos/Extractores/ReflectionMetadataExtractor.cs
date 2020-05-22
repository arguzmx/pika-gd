﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Modelo.Metadatos
{
    public class ReflectionMetadataExtractor<T> : IProveedorMetadatos<T>
    {
        public static string ObtieneTipoDato(Type t)
        {
            string type = "";
            switch (t)
            {
                case Type intType when intType == typeof(int):
                    return TipoDato.tInt32;

                case Type longype when longype == typeof(long):
                    return TipoDato.tInt64;

                case Type floatype when floatype == typeof(float):
                case Type decimalType when decimalType == typeof(decimal):
                    return TipoDato.tDouble;



                case Type datetimeType when datetimeType == typeof(DateTime):
                    return TipoDato.tDateTime;

                case Type boolType when boolType == typeof(bool):
                    return TipoDato.tBoolean;


                case Type stringType when stringType == typeof(string):
                    return TipoDato.tString;


                default:
                    break;
            }

            return type;

        }

        public async Task<MetadataInfo> Obtener()
        {

            var t = typeof(T);
            MetadataInfo info = new MetadataInfo() { Tipo = t.Name , FullName = t.FullName};
            List<Propiedad> properties = new List<Propiedad>();

            PropertyInfo[] props = t.GetProperties();

            foreach (PropertyInfo prop in props)
            {

                Propiedad foundProp = null;

                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is PropAttribute)
                    {
                        PropAttribute pa = (PropAttribute)attr;
                        foundProp = pa.GetProperty<T>();
                        foundProp.TipoDatoId = ReflectionMetadataExtractor<T>.ObtieneTipoDato(prop.PropertyType);
                    }
                }

                if (foundProp != null)
                {
                    foreach (object attr in attrs)
                    {

                        if (attr is TblAttribute)
                        {
                            TblAttribute ta = (TblAttribute)attr;
                            foundProp.AtributoTabla = ta.GetTable(prop.Name);
                        }

                        if (attr is ValidStringAttribute)
                        {
                            ValidStringAttribute a = (ValidStringAttribute)attr;
                            foundProp.ValidadorTexto = a.GetAttribute(prop.Name);
                        }


                        if (attr is ValidNumericAttribute)
                        {
                            ValidNumericAttribute a = (ValidNumericAttribute)attr;
                            foundProp.ValidadorNumero = a.GetAttribute(prop.Name);
                        }
                    }

                    properties.Add(foundProp);
                }

            }

            await Task.Delay(1);

            info.Propiedades = properties;
            return info;


        }

    }

    public static class MetadataHelper
    {
        public static AtributoTabla GetTable(this TblAttribute source, string Id)
        {
            return new AtributoTabla()
            {
                PropiedadId = Id,
                Incluir = source.IncludeIntable,
                IndiceOrdebnamiento = source.OrderIndex,
                IdTablaCliente = source.TableClientId,
                Alternable = source.Togglable,
                Visible = source.Visible
            };
        }
        public static Propiedad GetProperty<T>(this PropAttribute source)
        {

            return new Propiedad()
            {
                Nombre = source.I18nNameId,
                Id = source.I18nNameId,
                TipoDatoId = "",
                EsIdClaveExterna = source.IsFKId,
                Autogenerado = source.Autogenerated,
                EsIdRegistro = source.IsId,
                Ordenable = source.Orderable,
                IndiceOrdenamiento = source.OrderIndex,
                Requerido = source.Required,
                Buscable = source.Searchable,
                Visible = source.Visible,
                ControlHTML = source.HTMLControl,
                EsIndice = source.IskeyValue,
                ValorDefault = source.DefaultValue,
                EsIdJerarquia = source.IsHieId,
                EsIdPadreJerarquia = source.IsHieParentId,
                EsTextoJerarquia = source.IsHieText,
                EsFiltroJerarquia = source.IsHieFilter
            };
        }

        public static ValidadorTexto GetAttribute(this ValidStringAttribute source, string Id)
        {

            return new ValidadorTexto()
            {
                PropiedadId = Id,
                valordefault = source.defaulvalue,
                longmax = source.maxlen,
                longmin = source.minlen,
                regexp = source.regexp
            };
        }

        public static ValidadorNumero GetAttribute(this ValidNumericAttribute source, string Id)
        {

            return new ValidadorNumero()
            {
                PropiedadId = Id,
                valordefault = source.defaulvalue,
                max = source.max,
                min = source.min
            };
        }

    }
}
