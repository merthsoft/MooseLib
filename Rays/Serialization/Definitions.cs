using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;
public class Definitions
{
    [JsonPropertyName("actors")]
    public List<ActorDefinition> Actors { get; set; } = [];

    [JsonPropertyName("doors")]
    public List<DoorDefinition> Doors { get; set; } = [];

    [JsonPropertyName("special")]
    public List<SpecialDefinition> Special { get; set; } = [];

    [JsonPropertyName("objects")]
    public List<ObjectDefinition> Objects { get; set; } = [];
}
