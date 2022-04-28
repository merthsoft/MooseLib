using System.Text.Json.Serialization;

namespace RayMapEditor;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ObjectType { Static, Pickup, Overlay }
