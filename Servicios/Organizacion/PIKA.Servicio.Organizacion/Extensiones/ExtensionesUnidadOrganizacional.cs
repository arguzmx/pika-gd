using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Organizacion;

namespace PIKA.Servicio.Organizacion
{
    public static partial class Extensiones
    {
        public static UnidadOrganizacional Copia(this UnidadOrganizacional d)
        {
            if (d == null) return null;
            UnidadOrganizacional data = new UnidadOrganizacional()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Eliminada = d.Eliminada,
                DominioId = d.DominioId
            };

            return data;
        }

    }
}
