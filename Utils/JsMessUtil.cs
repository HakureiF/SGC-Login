using System.Text.Json;

namespace Seer.Utils;

internal class JsMessUtil<T>
{
    public string Type { get; }
    public T Data { get; }
    public string Signal { get; }

    private JsMessUtil(string type, T data, string signal)
    {
        Type = type;
        Data = data;
        Signal = signal;
    }

    public static string MessJson(string type, T data)
    {
        var messInstance = new JsMessUtil<T>(type, data, "ignore");
        return JsonSerializer.Serialize(messInstance);
    }

    public static string MessJson(string type, T data, string signal)
    {
        var messInstance = new JsMessUtil<T>(type, data, signal);
        return JsonSerializer.Serialize(messInstance);
    }
}