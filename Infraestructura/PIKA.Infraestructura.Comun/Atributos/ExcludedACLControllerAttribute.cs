using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.All)]
    public class ExcludedACLControllerAttribute : Attribute
    {
    }

}
