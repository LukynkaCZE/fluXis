using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users;

public class APIUserShort
{
    [JsonProperty("id")]
    public long ID { get; init; }

    [JsonProperty("username")]
    public string Username { get; init; } = null!;

    [JsonProperty("displayname")]
    public string? DisplayName { get; init; }

    [JsonProperty("country")]
    public string? CountryCode { get; init; }

    [JsonProperty("role")]
    public int Role { get; init; }

    [JsonIgnore]
    public string PreferredName => DisplayName ?? Username;

    [JsonIgnore]
    public string NameWithApostrophe
    {
        get
        {
            var name = PreferredName;
            if (name.EndsWith("s") || name.EndsWith("z"))
                return name + "'";

            return name + "'s";
        }
    }

    public static APIUserShort Dummy => new() { ID = -1, Username = "Dummy Player" };
    public static APIUserShort Default => new() { ID = -1, Username = "Player" };
    public static APIUserShort AutoPlay => new() { ID = 0, Username = "AutoPlay" };
}
