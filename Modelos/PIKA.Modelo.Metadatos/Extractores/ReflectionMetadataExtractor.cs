using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
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

                case Type datetimeTypeNulable when datetimeTypeNulable == typeof(DateTime?):
                    return TipoDato.tDateTime;

                case Type datetimeType when datetimeType == typeof(DateTime):
                    return TipoDato.tDateTime;

                case Type boolType when boolType == typeof(bool):
                    return TipoDato.tBoolean;

                case Type enumType when enumType.IsEnum:
                     return TipoDato.tInt32;

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
            MetadataInfo info = new MetadataInfo()
            {
                Tipo = t.Name,
                FullName = t.FullName,
                EntidadesVinculadas = new List<EntidadVinculada>(),
                CatalogosVinculados = new List<CatalogoVinculado>()
            };

            if (typeof(IEntidadReportes).IsAssignableFrom(typeof(T)))
            {
                T instance = Activator.CreateInstance<T>();
                ((IEntidadReportes)instance).Reportes.ForEach(r =>
                {
                    info.Reportes.Add(r);
                });

            }  

            object[] TypeAttrs = t.GetCustomAttributes(true);
            foreach (object attr in TypeAttrs) {

                if (attr is EntidadAttribute)
                {
                    
                    EntidadAttribute ea = ((EntidadAttribute)attr);
                    info.OpcionActivarDesativar = ea.OpcionActivar;
                    info.ColumnaActivarDesativar = ea.ColumnaActivar;
                    info.ElminarLogico = ea.EliminarLogico;
                    info.ColumaEliminarLogico = ea.Columna;
                    info.PaginadoRelacional = ea.PaginadoRelacional;
                    info.TokenApp = ea.TokenApp;
                    info.TokenMod = ea.TokenMod;
                    info.AsociadoMetadatos = ea.AsociadoMetadatos;
                    info.HabilitarSeleccion = ea.HabilitarSeleccion;
                    info.PermiteAltas = ea.PermiteAltas;
                    info.PermiteBajas = ea.PermiteBajas;
                    info.PermiteCambios = ea.PermiteCambios;
                    info.TipoSeguridad = ea.TipoSeguridad;
                    info.BuscarPorTexto = ea.BuscarPorTexto;
                    info.PermiteEliminarTodo = ea.PermiteEliminarTodo;
                }

                if (attr is EntidadVinculadaAttribute)
                {
                    var ev = ((EntidadVinculadaAttribute)attr);
                    info.EntidadesVinculadas.Add(new EntidadVinculada()
                    {
                        Cardinalidad = ev.Cardinalidad,
                        EntidadHijo = ev.Entidad,
                        PropiedadPadre = ev.PropiedadPadre,
                        PropiedadHijo = ev.PropiedadHijo,
                        TipoDespliegue = ev.TipoDespliegue,
                        HijoDinamico = ev.HijoDinamico, 
                        DiccionarioEntidadesVinculadas = ev.DiccionarioEntidadesVinculadas,
                        PropiedadIdMiembro = ev.PropiedadIdMiembro,
                        TokenSeguridad = ev.TokenSeguridad,
                        FiltroUI = ev.FiltroUI
                    });

                }

                if (attr is LinkCatalogoAttribute)
                {
                    
                    info.CatalogosVinculados.Add( ((LinkCatalogoAttribute)attr).Copia());

                }

                if (attr is LinkViewAttribute)
                {
                    info.VistasVinculadas.Add(((LinkViewAttribute)attr).Copia());
                }

                if (attr is MenuAttribute)
                {
                    info.Menus.Add(((MenuAttribute)attr).Copia());
                }
            }

            List<Propiedad> properties = new List<Propiedad>();

            PropertyInfo[] props = t.GetProperties();

            foreach (PropertyInfo prop in props)
            {
                
                object[] attrs = prop.GetCustomAttributes(true);

                List<PropAttribute> attrProps = new List<PropAttribute>();
                foreach (object attr in attrs)
                {
                    if (attr is PropAttribute)
                    {
                        attrProps.Add((PropAttribute)attr);
                    }
                }
             
                foreach (PropAttribute attrProp in attrProps)
                {
                    Propiedad foundProp = null;
                    PropAttribute pa = (PropAttribute)attrProp;
                    foundProp = pa.GetProperty<T>();
                    foundProp.TipoDatoId = ReflectionMetadataExtractor<T>.ObtieneTipoDato(prop.PropertyType);

                    foreach (object attr in attrs)
                    {

                        if (attr is LinkMetadatosAttribute)
                        {
                            var mattr = ((LinkMetadatosAttribute)attr);
                            foundProp.VinculoMetadatos = mattr.CampoMetadatos;
                        }

                        if (attr is JsonPropertyNameAttribute)
                        {
                            JsonPropertyNameAttribute ea = (JsonPropertyNameAttribute)attr;
                            foundProp.Id = ea.Name;
                            foundProp.Nombre = ea.Name;
                        }

                        if (attr is ListAttribute)
                        {
                            ListAttribute la = (ListAttribute)attr;
                            foundProp.AtributoLista = la.GetLista(prop.Name);

                        }

                        if (attr is EventAttribute)
                        {
                            EventAttribute ea = (EventAttribute)attr;
                            foundProp.AtributosEvento.Add(ea.GetEvento(prop.Name));
                        }

                        if (attr is VistaUIAttribute)
                        {
                            VistaUIAttribute va = (VistaUIAttribute)attr;
                            if (va.Entidad == pa.Entidad)
                            {
                                foundProp.AtributosVistaUI.Add(va.GetUI(prop.Name));
                            }
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

                        if (attr is LinkViewParameterAttribute)
                        {
                            foundProp.ParametroLinkVista = ((LinkViewParameterAttribute)attr).Copia();
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

        public static AtributoEvento GetEvento(this EventAttribute source, string Id)
        {
            return new AtributoEvento()
            {
                PropiedadId = Id,
                Entidad = source.Entidad,
                Parametro = source.Parametros,
                Evento = source.Evento,
                Operacion = source.Operacion

            };
        }

        public static AtributoVistaUI GetUI(this VistaUIAttribute a, string Id) {

            return new AtributoVistaUI()
            {
                PropiedadId = Id,
                Accion = a.Accion,
                Control = a.Control, 
                Plataforma = a.Plataforma,
                Entidad = a.Entidad
            };
        }


        public static AtributoLista GetLista(this ListAttribute source, string Id)
        {
            return new AtributoLista()
            {
                PropiedadId = Id,
                Entidad = source.Entidad,
                DatosRemotos = source.DatosRemotos,
                TypeAhead = source.TypeAhead,
                Default = source.Default,
                OrdenarAlfabetico = source.OrdenarAlfabetico,
                ValoresCSV = source.ValoresCSV,
                EsListaTemas = source.EsListaTemas,
                FiltroBusqueda = source.FiltroBusqueda
            };
        }

    
        public static Propiedad GetProperty<T>(this PropAttribute source)
        {

            return new Propiedad()
            {
                Nombre = source.Id,
                Id = source.Id,
                TipoDatoId = "",
                Ordenable = source.Orderable,
                IndiceOrdenamiento = source.OrderIndex,
                Requerido = source.Required,
                Buscable = source.Searchable,
                Visible = source.Visible,
                ValorDefault = source.DefaultValue,
                EsIdRegistro = source.isId,
                IndiceOrdenamientoTabla = source.TableOrderIndex, 
                MostrarEnTabla = source.ShowInTable,
                AlternarEnTabla = source.ToggleInTable,
                Contextual = source.Contextual,
                IdContextual = source.IdContextual,
                Etiqueta  = source.isLabel,
                EsIdJerarquia = source.HieId,
                EsTextoJerarquia = source.HIeName,
                EsIdRaizJerarquia = source.HieRoot,
                EsFiltroJerarquia = source.HieParent,
                Entidad = source.Entidad,
                Formato = source.Formato
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
                min = source.min, UtilizarMax = source.usemax, 
                UtilizarMin = source.usemin
            };
        }

    }
}
