using Google.Protobuf.WellKnownTypes;

namespace GameLibrary.IGDB;

public static class TimestampExtensions
{
    public static DateTimeOffset? ToDateTimeOffsetOrNull(this Timestamp? timestamp)
    {
        if (timestamp is null)
        {
            return null;
        }

        try
        {
            return timestamp.ToDateTimeOffset();
        }
        catch
        {
            return null;
        }
    }
}
