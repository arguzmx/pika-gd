﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ValidStringAttribute : Attribute
    {

        private int _minlen;
        private int _maxlen;
        private string _defaulvalue;
        private string _regexp;
        public ValidStringAttribute(int minlen = 0, int maxlen = 0, string defaulvalue = "", string regexp = "")
        {
            _minlen = minlen;
            _maxlen = maxlen;
            _defaulvalue = defaulvalue;
            _regexp = regexp;
        }

        public int minlen { get { return _minlen; } }
        public int maxlen { get { return _maxlen; } }
        public string defaulvalue { get { return _defaulvalue; } }
        public string regexp { get { return _regexp; } }

    }

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