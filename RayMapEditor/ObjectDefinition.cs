using System.Text.Json.Serialization;

namespace RayMapEditor;
public class ObjectDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!; 
    
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("type")]
    public ObjectType Type { get; set; }

    [JsonPropertyName("blocking")]
    public bool Blocking { get; set; } = false;

    [JsonPropertyName("frames")]
    public List<Frame> Frames { get; set; } = new();

    public override string ToString()
        => $"{Index}: {Name}";
}