using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public static partial class Extensiones

    {
        public static MedioContacto Copia(this MedioContacto d)
        {
            var c = new MedioContacto()
            {
                Activo = d.Activo,
                Id = d.Id,
                Medio = d.Medio,
                Notas = d.Notas,
                OrigenId = d.OrigenId,
                Prefijo = d.Prefijo,
                Principal = d.Principal,
                Sufijo = d.Sufijo,
                TipoFuenteContactoId = d.TipoFuenteContactoId,
                TipoMedioId = d.TipoMedioId,
                TipoOrigenId = d.TipoOrigenId
            };

            if (d.TipoFuenteContacto != null) {
                c.TipoFuenteContacto = d.TipoFuenteContacto.Copia();
            }


            if (d.TipoMedio != null)
            {
                c.TipoMedio = d.TipoMedio.Copia();
            }

            if (d.Horarios != null) {
                c.Horarios = d.Horarios.Select(x => x.Copia()).ToList();
            }

            return c;
        }




        public static HorarioMedioContacto Copia(this HorarioMedioContacto d)
        {

            var c = new HorarioMedioContacto()
            {
                DiaSemana = d.DiaSemana,
                Fin = d.Fin,
                Id = d.Id,
                Inicio = d.Inicio,
                MedioContactoId = d.MedioContactoId,
                SinHorario = d.SinHorario

            };
            return c;

        }

    }

    
}
