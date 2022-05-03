using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;
public class Definitions
{
    [JsonPropertyName("actors")]
    public List<ActorDefinition> Actors { get; set; } = new();

    [JsonPropertyName("doors")]
    public List<DoorDefinition> Doors { get; set; } = new();

    [JsonPropertyName("special")]
    public List<SpecialDefinition> Special { get; set; } = new();

    [JsonPropertyName("objects")]
    public List<ObjectDefinition> Objects { get; set; } = new();
}
