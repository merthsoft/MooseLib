using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;

public class SpecialDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("index")]
    public int Index { get; set; }
}
