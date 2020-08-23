using Microsoft.AspNetCore.Components.Forms;
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
    }
}
