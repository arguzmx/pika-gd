using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Eventos
{
    public interface IFuenteEvento<T>
    {

        string CreaPayload();
        string TextoHumano();

    }
}
