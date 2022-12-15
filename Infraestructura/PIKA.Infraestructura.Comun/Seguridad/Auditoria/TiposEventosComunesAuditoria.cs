using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad.Auditoria
{

    public enum EventosComunesAuditables
    {
        Crear = 1, Actualizar = 2, Eliminar = 3, Leer = 4, Purgar = 5
    }

    public enum CodigoComunesFalla
    {
        Desconocido = 0,
        NoAutorizado = 1,
        DatosIncorectos = 2,
        DatosSesionIncorrectos = 3
    }


}
