using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Modelo.Aplicacion.Tareas;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public static class ExtensionesAplicacion
    {

        public static DateTime? SiguienteFechaEjecucion(this TareaAutomatica entity)
        {

            DateTime? ejecucion = null;
            bool programada;
            switch (entity.Periodo)
            {
                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Unico:
                    ejecucion = entity.FechaHoraEjecucion;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaSemana:
                    ejecucion = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);

                    programada = false;
                    // Si la el día de la semana es hoy verifica que la hora sea posterior
                    if (DateTime.Now.DayOfWeek.GetHashCode() == entity.DiaSemana.Value)
                    {
                        if (ejecucion > DateTime.UtcNow)
                        {
                            programada = true;
                        }
                    }

                    // Recorre la programación hasta el siguiente día de la semana
                    if (!programada)
                    {
                        while (!programada)
                        {
                            ejecucion = ejecucion.Value.AddDays(1);
                            if (ejecucion.Value.DayOfWeek.GetHashCode() == entity.DiaSemana.Value)
                            {
                                programada = true;
                            }
                        }
                    }
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Hora:
                    ejecucion = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                    while (ejecucion < DateTime.UtcNow)
                    {
                        ejecucion.Value.AddHours(entity.Intervalo.Value);
                    }
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaMes:
                    int diasmes = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);

                    if (diasmes < entity.DiaMes.Value)
                    {
                        ejecucion = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, diasmes, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    }
                    else
                    {
                        ejecucion = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, entity.DiaMes.Value, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    }

                    // Si la facha actual supera la programación se pasa al siguiente mes
                    if (DateTime.UtcNow > ejecucion)
                    {
                        diasmes = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1);
                        if (diasmes < entity.DiaMes.Value)
                        {
                            ejecucion = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1, diasmes, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                        }
                        else
                        {
                            ejecucion = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1, entity.DiaMes.Value, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                        }
                    }
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Diario:
                    ejecucion = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    if (DateTime.UtcNow > ejecucion)
                    {
                        ejecucion = ejecucion.Value.AddDays(1);
                    }
                    break;

            }

            return ejecucion;
        }


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
                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Continuo:
                    d.EtiquetaIntervalo = $"{d.TareaEjecucionContinuaMinutos}";
                    d.EtiquetaPeriodo = "listas.tareaautomatica.continuo";
                    break;

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
