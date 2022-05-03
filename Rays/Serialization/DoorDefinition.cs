using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;

public class DoorDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("key")]
    public int Key { get; set; }
}
