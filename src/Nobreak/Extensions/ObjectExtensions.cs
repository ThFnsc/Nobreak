using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class ObjectExtensions
    {
        public static List<KeyValuePair<string, object>> GetKeyValuePairs(this object input) =>
            input.GetType().GetProperties().Select(prop => new KeyValuePair<string, object>(prop.Name, prop.GetValue(input))).ToList();

        public static string ToJSON(this object input) =>
            JsonConvert.SerializeObject(input);

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
