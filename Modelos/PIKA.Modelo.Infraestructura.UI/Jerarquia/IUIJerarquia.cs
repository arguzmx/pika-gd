using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Infraestructura.UI
{
    public interface IUIJerarquia
    {
        ModeloJerarquia Modelo(string Tipo);
    }

}
