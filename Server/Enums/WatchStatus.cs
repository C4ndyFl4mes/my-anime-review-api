using System.Text.Json.Serialization;

namespace Server.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WatchStatus
{
    Planned,
    Watching,
    Completed,
    OnHold,
    Dropped
}