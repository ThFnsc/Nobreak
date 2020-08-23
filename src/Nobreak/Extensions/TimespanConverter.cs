using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Nobreak.Extensions
{
    public class TimespanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, [AllowNull] TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            TimeSpan.FromSeconds(double.Parse(reader.Value.ToString()));

        public override void WriteJson(JsonWriter writer, [AllowNull] TimeSpan value, JsonSerializer serializer) =>
            writer.WriteValue(value.TotalSeconds);
    }
}
