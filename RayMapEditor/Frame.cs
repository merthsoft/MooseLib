using System.Text.Json.Serialization;

namespace RayMapEditor;

public class Frame
{
    [JsonPropertyName("frame")]
    public int Index { get; set; }

    [JsonPropertyName("minTime")]
    public int MinTime { get; set; }

    [JsonPropertyName("maxTime")]
    public int MaxTime { get; set; }

    public override string ToString()
        => $"{Index} | {MinTime} - {MaxTime}";
}
