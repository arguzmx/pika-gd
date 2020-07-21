using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad
{
    public static partial class Extensiones
    {

        public static PropiedadesUsuario Copia(this PropiedadesUsuario d)
        {
            if (d == null) return null;
            var r = new PropiedadesUsuario()
            {
                UsuarioId = d.UsuarioId,
                email = d.email,
                password = "",
                username = d.username,
                email_verified = d.email_verified,
                estadoid = d.estadoid,
                family_name = d.family_name,
                generoid = d.generoid,
                given_name = d.given_name,
                gmt = d.gmt,
                gmt_offset = d.gmt_offset,
                middle_name = d.middle_name,
                name = d.name,
                nickname = d.nickname,
                paisid = d.paisid,
                picture = "",
                updated_at = d.updated_at,
            };

            return r;
        }
    }
}
