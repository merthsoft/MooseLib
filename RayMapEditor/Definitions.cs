using System.Text.Json.Serialization;

namespace RayMapEditor;
public class Definitions
{
    [JsonPropertyName("objects")]
    public List<ObjectDefinition> Objects { get; set; } = new();
}
