using PIKA.Modelo.Aplicacion.Tareas;
using System;


namespace PIKA.GD.API.Servicios.TareasAutomaticas
{
    public static class ExtensionesTarea
    {
        public static DateTime? SiguienteFechaEjecucion(this TareaAutomatica entity, DateTime? ultimaEjecucion = null)
        {
            if (!ultimaEjecucion.HasValue)
            {
                ultimaEjecucion = DateTime.UtcNow;
            }

            if (!entity.HoraEjecucion.HasValue)
            {
                entity.HoraEjecucion = new DateTime(2001, 1, 1, 0, 0, 0);
            }

            DateTime? ejecucion = null;
            bool programada;
            switch (entity.Periodo)
            {
                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Continuo:
                    ejecucion = DateTime.UtcNow;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.Unico:
                    ejecucion = entity.FechaHoraEjecucion;
                    break;

                case Infraestructura.Comun.Tareas.PeriodoProgramacion.DiaSemana:
                    ejecucion = new DateTime(ultimaEjecucion.Value.Year, ultimaEjecucion.Value.Month, ultimaEjecucion.Value.Day, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);

                    programada = false;
                    // Si la el día de la semana es hoy verifica que la hora sea posterior
                    if (DateTime.Now.DayOfWeek.GetHashCode() == entity.DiaSemana.Value)
                    {
                        if (ejecucion > ultimaEjecucion.Value)
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
                    ejecucion = new DateTime(ultimaEjecucion.Value.Year, ultimaEjecucion.Value.Month, ultimaEjecucion.Value.Day, 0, 0, 0);
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
                    if (ultimaEjecucion.Value > ejecucion)
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
                    ejecucion = new DateTime(ultimaEjecucion.Value.Year, ultimaEjecucion.Value.Month, ultimaEjecucion.Value.Day, entity.HoraEjecucion.Value.Hour, entity.HoraEjecucion.Value.Minute, 0);
                    if (ultimaEjecucion.Value > ejecucion)
                    {
                        ejecucion = ejecucion.Value.AddDays(1);
                    }
                    break;

            }

            return ejecucion;
        }

    }
}
