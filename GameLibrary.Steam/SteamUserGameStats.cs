using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameLibrary.Steam;


public class SteamUserGameReport
{
    [JsonPropertyName("response")] public SteamUserGameResponse Response { get; set; } = null!;

    public class SteamUserGameResponse
    {
        [JsonPropertyName("game_count")] public int GameCount { get; set; }
        [JsonPropertyName("games")] public IEnumerable<SteamUserGameStats> Games { get; set; } = null!;
        public class SteamUserGameStats
        {
            [JsonPropertyName("appid")] public long AppId { get; set; }
            [JsonPropertyName("rtime_last_played")]
            [JsonConverter(typeof(TimestampConverter))]
            public DateTimeOffset? LastPlayed { get; set; }
            [JsonPropertyName("playtime_forever")]
            [JsonConverter(typeof(PlaytimeConverter))]
            public TimeSpan Playtime { get; set; }
        }
    }
}

public class PlaytimeConverter: JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeSpan.FromMinutes(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value.TotalMinutes);
    }
}

public class TimestampConverter: JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetInt64();
        return value == 0 ? null : DateTimeOffset.FromUnixTimeSeconds(value);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value?.ToUnixTimeSeconds() ?? 0);
    }
}
