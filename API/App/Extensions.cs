using System.Text.Json;
using System.Text.Json.Serialization;

namespace PowerApps.App;

/// <summary>Plėtiniai</summary>
public static class Extensions {

}


/// <summary>Datos formatavimas</summary>
public class CustomDateTimeConverter : JsonConverter<DateTime> {
	/// <summary></summary><param name="reader"></param><param name="typeToConvert"></param><param name="options"></param><returns></returns>
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => DateTime.TryParse(reader.GetString(), out var dt) ? dt : default;
	/// <summary></summary><param name="writer"></param><param name="value"></param><param name="options"></param>
	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
}

/// <summary>Datos formatavimas</summary>
public class CustomDateOnlyConverter : JsonConverter<DateOnly> {
	/// <summary></summary><param name="reader"></param><param name="typeToConvert"></param><param name="options"></param><returns></returns>
	public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => DateOnly.TryParse(reader.GetString(), out var dt) ? dt : default;
	/// <summary></summary><param name="writer"></param><param name="value"></param><param name="options"></param>
	public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
}


/// <summary>Skaičiaus+teksto serializavimas</summary>
public class CustomIntStringTupleConverter : JsonConverter<(int code, string message)> {
	/// <summary></summary><param name="reader"></param><param name="typeToConvert"></param><param name="options"></param><returns></returns>
	public override (int code, string message) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) { throw new NotImplementedException(); }
	/// <summary></summary><param name="writer"></param><param name="value"></param><param name="options"></param>
	public override void Write(Utf8JsonWriter writer, (int code, string message) value, JsonSerializerOptions options) { writer.WriteStartArray(); writer.WriteNumberValue(value.code); writer.WriteStringValue(value.message); writer.WriteEndArray(); }
}