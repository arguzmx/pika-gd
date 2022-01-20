using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Modelo.Aplicacion.Tareas;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public static class ExtensionesAplicacion
    {
        public static TareaAutomatica Copia(this TareaAutomatica d)
        {
            return new TareaAutomatica()
            {
                Id = d.Id,
                CodigoError = d.CodigoError,
                Duracion = d.Duracion,
                Ensamblado = d.Ensamblado,
                Exito = d.Exito,
                FechaHoraEjecucion = d.FechaHoraEjecucion,
                Nombre = d.Nombre,
                Intervalo = d.Intervalo,
                OrigenId = d.OrigenId,
                Periodo = d.Periodo,
                ProximaEjecucion = d.ProximaEjecucion,
                TipoOrigenId = d.TipoOrigenId,
                UltimaEjecucion = d.UltimaEjecucion
            };
        }

        public static TareaAutomatica EstableceEtiquetas(this TareaAutomatica d, int GMTOffset)
        {
            d.FechaHoraEjecucion = d.FechaHoraEjecucion.Value.AddMinutes(GMTOffset);
            d.EtiquetaFecha = d.FechaHoraEjecucion.Value.ToString("HH:mm:ss");
            switch (d.Periodo)
            {
                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Hora:
                    d.EtiquetaIntervalo = $"{d.Intervalo}";
                    d.EtiquetaPeriodo = "listas.tareaautomatica.hora";
                    d.HoraEjecucion = d.FechaHoraEjecucion;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Unico:
                    d.EtiquetaIntervalo = $"listas.tareaautomatica.unico";
                    d.EtiquetaPeriodo = "listas.tareaautomatica.unico";
                    d.EtiquetaFecha = d.FechaHoraEjecucion.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaSemana:
                    d.EtiquetaPeriodo = "listas.tareaautomatica.diasemana";
                    switch (d.Intervalo)
                    {
                        case 0:
                            d.EtiquetaIntervalo = "listas.tareaautomatica.domingo";
                            break;

                        case 1:
                            d.EtiquetaIntervalo = "listas.tareaautomatica.lunes";
                            break;

                        case 2:
                            d.EtiquetaIntervalo = "listas.tareaautomatica.martes";
                            break;

                        case 3:
                            d.EtiquetaIntervalo = "listas.tareaautomatica.miercoles";
                            break;

                        case 4:
                            d.EtiquetaIntervalo = "listas.tareaautomatica.jueves";
                            break;

                        case 5:
                            d.EtiquetaIntervalo = "listas.tareaautomatica.viernes";
                            break;

                        case 6:
                            d.EtiquetaIntervalo = "listas.tareaautomatica.sabado";
                            break;

                    }
                    d.DiaSemana = d.Intervalo;
                    d.HoraEjecucion = d.FechaHoraEjecucion;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaMes:
                    d.EtiquetaPeriodo = "listas.tareaautomatica.diames";
                    d.EtiquetaIntervalo = d.Intervalo.ToString();
                    d.DiaMes = d.Intervalo;
                    d.HoraEjecucion = d.FechaHoraEjecucion;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Diario:
                    d.EtiquetaPeriodo = "listas.tareaautomatica.diario";
                    d.EtiquetaIntervalo = d.Intervalo.ToString();
                    d.HoraEjecucion = d.FechaHoraEjecucion;
                    break;

            }

            return d;
        }

        public static Plugin CopiaPlugin(this Plugin d)
        {
            return new Plugin()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Gratuito=d.Gratuito,
                Eliminada=d.Eliminada
            };
        }
        public static VersionPlugin CopiaVersionPlugin(this VersionPlugin d)
        {
            return new VersionPlugin()
            {
                Id = d.Id,
                PluginId=d.PluginId,
                RequiereConfiguracion=d.RequiereConfiguracion,
                URL=d.URL
            };
        }
        public static PluginInstalado CopiaPluginInstalado(this PluginInstalado d)
        {
            return new PluginInstalado()
            {
                PLuginId=d.PLuginId,
                VersionPLuginId=d.VersionPLuginId,
                Activo=d.Activo,
                FechaInstalacion=d.FechaInstalacion
            };
        }
      
    }
}
