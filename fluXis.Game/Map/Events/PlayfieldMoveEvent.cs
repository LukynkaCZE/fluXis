using fluXis.Game.Map.Structures;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Events;

public class PlayfieldMoveEvent : ITimedObject
{
    [JsonProperty("time")]
    public float Time { get; set; }

    [JsonProperty("x")]
    public float OffsetX { get; set; }

    [JsonProperty("duration")]
    public float Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }
}
