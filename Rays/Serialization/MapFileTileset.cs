using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;

public class MapFileTileset
{
    [JsonPropertyName("firstgid")]
    public int FirstGId { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = "";
}