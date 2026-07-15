using System.Text.Json.Serialization;

namespace Server.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BugState
{
    [JsonStringEnumMemberName("Pending")]
    Pending,
    [JsonStringEnumMemberName("Planned")]
    Planned,
    [JsonStringEnumMemberName("In Progress")]
    InProgress,
    [JsonStringEnumMemberName("Completed")]
    Completed,
    [JsonStringEnumMemberName("Rejected")]
    Rejected
}