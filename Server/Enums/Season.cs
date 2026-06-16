using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Server.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Season
{
    [JsonStringEnumMemberName("winter")]
    Winter,
    [JsonStringEnumMemberName("spring")]
    Spring,
    [JsonStringEnumMemberName("summer")]
    Summer,
    [JsonStringEnumMemberName("fall")]
    Fall
}