using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class ValidStringAttribute : Attribute
    {

        public const string REGEXP_NUMBERS = @"[0-9]*";
        public const string REGEXP_USERPASS = @"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}";
        public const string REGEXP_EMAIL = @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}";

        //^$

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

}
