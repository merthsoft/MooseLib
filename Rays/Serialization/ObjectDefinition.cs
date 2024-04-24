using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;

public class ObjectDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("type")]
    public ObjectType Type { get; set; }

    [JsonPropertyName("blocking")]
    public bool Blocking { get; set; } = false;

    [JsonPropertyName("frames")]
    public List<Frame> Frames { get; set; } = [];

    [JsonIgnore]
    public int FirstFrameIndex => Frames[0].Index;

    public override string ToString()
        => Name + (Blocking ? "*" : "");
}