using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static TipoDisposicionDocumental Copia(this TipoDisposicionDocumental a)
        {
            if (a == null) return null;
            return new TipoDisposicionDocumental()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                DominioId = a.DominioId,
                UOId = a.UOId
            };
        }
    }
}
