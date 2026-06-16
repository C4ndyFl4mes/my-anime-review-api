using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Server.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AgeRating
{
    [JsonStringEnumMemberName("G - All Ages")]
    G,
    [JsonStringEnumMemberName("PG - Children")]
    PG,
    [JsonStringEnumMemberName("PG-13 - Teens 13 or older")]
    PG13,
    [JsonStringEnumMemberName("R - 17+ (violence & profanity)")]
    R17,
    [JsonStringEnumMemberName("R+ - Mild Nudity")]
    RPlus,
    [JsonStringEnumMemberName("Rx - Hentai")]
    RxHentai
}