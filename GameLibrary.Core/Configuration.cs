using System.Text;
using System.Text.Json;

namespace GameLibrary.Core;

public class Configuration
{
    public const string ApplicationName = "GameLibrary";
    private static readonly string ConfigFolderPath =
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
    public static readonly string DataFolderPath =
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationName);

    private static readonly string DefaultConfigurationPath = Path.Join(ConfigFolderPath, "config.json");
    private static readonly JsonWriterOptions JsonWriterOptions = new()
    {
        Indented = true
    };
    private static readonly JsonReaderOptions JsonReaderOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };
    private static readonly Dictionary<string, Type> _types = new();

    private readonly Dictionary<string, object?> _objects = new();
    private readonly string _path;

    public Configuration(string path)
    {
        _path = path;
        Read();
    }

    public Configuration() : this(DefaultConfigurationPath)
    {
    }

    public static void Register<T>(string key) where T: class
    {
        _types[key] = typeof(T);
    }

    public static void Register<T>() where T : class
    {
        Register<T>(typeof(T).Name);
    }

    public T Get<T>(string key) where T : class
    {
        return (T)_objects[key]!;
    }

    public T Get<T>() where T : class
    {
        return Get<T>(typeof(T).Name);
    }

    public void Set<T>(string key, T value) where T : class
    {
        _objects[key] = value;
    }

    private void Read()
    {
        var json = File.ReadAllText(_path);
        _objects.Clear();
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json), JsonReaderOptions);

        reader.Read();
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Invalid configuration JSON");
        }

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var key = reader.GetString()!;
                if (_types.TryGetValue(key, out var type))
                {
                    var value = JsonSerializer.Deserialize(ref reader, type);
                    _objects[key] = value;
                }
                else
                {
                    JsonSerializer.Deserialize(ref reader, typeof(Dictionary<string, object>));
                }
            }
        }
    }

    public void Write()
    {
        File.WriteAllText(_path, ToJson());
    }

    public string ToJson()
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, JsonWriterOptions);

        writer.WriteStartObject();
        foreach (var pair in _objects)
        {
            writer.WritePropertyName(pair.Key);
            JsonSerializer.Serialize(writer, pair.Value);
        }
        writer.WriteEndObject();
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static string EnsureCacheDirectory(string subdirectory)
    {
        var path = Path.Join(DataFolderPath, "cache", subdirectory);
        Directory.CreateDirectory(path);
        return path;
    }
}
