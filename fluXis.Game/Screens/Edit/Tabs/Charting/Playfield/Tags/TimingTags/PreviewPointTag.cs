using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class PreviewPointTag : EditorTag
{
    [Resolved]
    private EditorValues values { get; set; }

    public override Colour4 TagColour => Colour4.FromHex("FDD27F");

    public PreviewPointTag(EditorTagContainer parent)
        : base(parent, new PreviewPointObject())
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Text.Text = "Preview";
    }

    protected override void Update()
    {
        TimedObject.Time = values.MapInfo.Metadata.PreviewTime;
        base.Update();
    }

    // placeholder class for the preview point
    private class PreviewPointObject : ITimedObject
    {
        public float Time { get; set; }
    }
}
