using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static TipoValoracionDocumental Copia(this TipoValoracionDocumental a)
        {
            if (a == null) return null;
            return new TipoValoracionDocumental()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                DominioId = a.DominioId,
                UOId = a.UOId
            };
        }
    }
}
