using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Middlewares
{
    public class APIErrorDetail
    {

        public APIErrorDetail()
        {
            Message = "";
            StatusCode = 500;
        }
        public string Message { get; set; }

        public int StatusCode { get; set; }


        public override string ToString()
        {
            return $"{StatusCode} {Message}";
        }

    }
}
