using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Atributos
{

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ValidNumericAttribute : Attribute
    {

        private float _min;
        private float _max;
        private float _defaulvalue;

        public ValidNumericAttribute(float min = 0, float max = 0, float defaulvalue = float.NaN)
        {
            _min = min;
            _max = max;
            _defaulvalue = defaulvalue;
        }

        public float min { get { return _min; } }
        public float max { get { return _max; } }
        public float defaulvalue { get { return _defaulvalue; } }

    }

}
