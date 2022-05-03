using System.Text.Json.Serialization;

namespace Merthsoft.Moose.Rays.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ObjectType { Static, Pickup, Overlay }
