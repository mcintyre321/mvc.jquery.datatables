using System;
using Newtonsoft.Json;

namespace Mvc.JQuery.Datatables.Serialization
{
    public class RawConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Raw);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null && value.GetType() == typeof(Raw))
            {
                writer.WriteRawValue(value.ToString());
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }
}