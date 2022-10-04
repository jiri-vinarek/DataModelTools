using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Extractor.Dto;

public class ConcatenatingLineConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            return serializer.Deserialize(reader, objectType);
        }
        if (reader.TokenType == JsonToken.StartArray)
        {
            JArray array = JArray.Load(reader);
            var values =  array.Select(t => t.Value<string>());
            return string.Join("\n", values);
        }

        throw new NotImplementedException($"The {nameof(ConcatenatingLineConverter)} expects string or array of strings in this context.");
    }
    
    public override bool CanRead
    {
        get { return true; }
    }
    
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string) || typeToConvert == typeof(List<string>);
    }
}
