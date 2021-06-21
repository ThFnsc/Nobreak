using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object input, bool indented = false)
        {
            var options = new JsonSerializerOptions { WriteIndented = indented };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Serialize(input, options);
        }

        public static IDictionary<string, object> ConvertToDictionary(this object input)
        {
            if (input is IDictionary<string, object> asDict)
                return asDict;
            var dict = new Dictionary<string, object>();
            foreach (var prop in input.GetType().GetProperties())
                dict[prop.Name] = prop.GetValue(input);
            return dict;
        }
    }
}
