using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;

public class ActorDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
