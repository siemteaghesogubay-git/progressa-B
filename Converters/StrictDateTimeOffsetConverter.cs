using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace progressa.Converters;

public sealed class StrictDateTimeOffsetConverter
    : JsonConverter<DateTimeOffset>
{
    // Accepterar:
    // 2026-11-09T18:00:00+01:00
    // 2026-11-09T17:00:00Z
    // Även bråkdelar av sekunder tillåts, upp till 7 siffror.
    private static readonly Regex TimestampPattern = new(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}" +
        @"(?:\.\d{1,7})?(?:Z|[+-]\d{2}:\d{2})$",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));

    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                "Datum och tid måste anges som en textsträng.");
        }

        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value) ||
            !TimestampPattern.IsMatch(value))
        {
            throw new JsonException(
                "Ange datum och tid med tidszon, exempelvis " +
                "2026-11-09T18:00:00+01:00 eller " +
                "2026-11-09T17:00:00Z.");
        }

        if (!reader.TryGetDateTimeOffset(out var timestamp))
        {
            throw new JsonException(
                "Datum, klockslag eller tidszonsförskjutning är ogiltigt.");
        }

        return timestamp;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options)
    {
        // Returnera alltid DateTimeOffset-värden som UTC med Z.
        writer.WriteStringValue(
            value.UtcDateTime.ToString(
                "O",
                CultureInfo.InvariantCulture));
    }
}