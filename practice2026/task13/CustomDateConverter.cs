using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace task13
{
    public class CustomDateConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParseExact(reader.GetString(),Format,null,DateTimeStyles.None,out var date))
            {
                return date;
            }
            throw new JsonException($"Неверный формат даты. Ожидается {Format}");
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
