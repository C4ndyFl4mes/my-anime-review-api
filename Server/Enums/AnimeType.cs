using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Server.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AnimeType
{
    [JsonStringEnumMemberName("TV")]
    TV,
    [JsonStringEnumMemberName("OVA")]
    OVA,
    [JsonStringEnumMemberName("ONA")]
    ONA,
    [JsonStringEnumMemberName("CM")]
    CM,
    [JsonStringEnumMemberName("PV")]
    PV,
    [JsonStringEnumMemberName("Special")]
    Special,
    [JsonStringEnumMemberName("TV Special")]
    TVSpecial,
    [JsonStringEnumMemberName("Movie")]
    Movie,
    [JsonStringEnumMemberName("Music")]
    Music
}