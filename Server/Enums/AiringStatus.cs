using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Server.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AiringStatus
{
    [JsonStringEnumMemberName("Not yet aired")]
    NotYetAired,
    [JsonStringEnumMemberName("Currently Airing")]
    CurrentlyAiring,
    [JsonStringEnumMemberName("Finished Airing")]
    FinishedAiring
}