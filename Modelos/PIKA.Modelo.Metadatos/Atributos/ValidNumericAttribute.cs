using Microsoft.AspNetCore.Builder;
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
        private bool _usemax;
        private bool _usemin;
        private float _defaulvalue;

        public ValidNumericAttribute(float min = 0, float max = 0, float defaulvalue = float.NaN, bool usemin=false, bool usemax=false)
        {
            _min = min;
            _max = max;
            _defaulvalue = defaulvalue;
            _usemax = usemax;
            _usemin = usemin;
        }

        public float min { get { return _min; } }
        public float max { get { return _max; } }
        public bool usemax { get { return _usemax; } }
        public bool usemin { get { return _usemin; } }
        public float defaulvalue { get { return _defaulvalue; } }

    }

}
