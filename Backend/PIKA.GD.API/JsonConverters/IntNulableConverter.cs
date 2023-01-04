using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PIKA.GD.API.JsonConverters
{
    public class IntNulableConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // int entero;
            int? input;
            try
            {
                input = reader.GetInt32();
                return input== null ? default(int?) : input;    
            }
            catch (Exception)
            {
            }
            
            //if (!string.IsNullOrEmpty(input))
            //{
            //    if(int.TryParse(input, out entero))
            //    {
            //        return entero;
            //    }
            //}

            return null;
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {   
            writer.WriteStringValue(value.ToString());
        }
    }
}
