using System.Linq;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect;

public partial class EditorEffectContainer : Container
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    private Box flashUnderlay;

    public Container<EditorFlashEvent> Flashes { get; private set; }
    public Container<EditorLaneSwitchEvent> LaneSwitches { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            flashUnderlay = new Box
            {
                Width = 14,
                RelativeSizeAxes = Axes.Y,
                Alpha = 0,
                Colour = FluXisColors.Background1,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Left = 8 }
            },
            Flashes = new Container<EditorFlashEvent>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Y = -EditorHitObjectContainer.HITPOSITION,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Left = 10 }
            },
            LaneSwitches = new Container<EditorLaneSwitchEvent>
            {
                RelativeSizeAxes = Axes.Both,
                Y = -EditorHitObjectContainer.HITPOSITION
            }
        };

        loadEvents();

        changeHandler.OnKeyModeChanged += _ =>
        {
            ClearAll();
            loadEvents();
        };

        values.FlashUnderlay.BindValueChanged(val => flashUnderlay.FadeTo(val.NewValue ? 1 : 0, 200), true);
        values.FlashUnderlayColor.BindValueChanged(val => flashUnderlay.Colour = val.NewValue, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        values.MapEvents.FlashEventAdded += AddFlash;
        values.MapEvents.LaneSwitchEventAdded += AddLaneSwitch;

        values.MapEvents.FlashEventRemoved += flash =>
        {
            var editorFlash = Flashes.FirstOrDefault(f => f.FlashEvent == flash);
            if (editorFlash != null)
                Flashes.Remove(editorFlash, true);
        };

        values.MapEvents.LaneSwitchEventRemoved += ls =>
        {
            var editorLs = LaneSwitches.FirstOrDefault(l => l.Event == ls);
            if (editorLs != null)
                LaneSwitches.Remove(editorLs, true);
        };
    }

    private void loadEvents()
    {
        foreach (var flashEvent in values.MapEvents.FlashEvents)
            AddFlash(flashEvent);

        foreach (var laneSwitch in values.MapEvents.LaneSwitchEvents)
            AddLaneSwitch(laneSwitch);
    }

    public void AddFlash(FlashEvent flash)
    {
        Flashes.Add(new EditorFlashEvent { FlashEvent = flash });
    }

    public void AddLaneSwitch(LaneSwitchEvent ls)
    {
        LaneSwitches.Add(new EditorLaneSwitchEvent { Event = ls });
    }

    public void ClearAll()
    {
        Flashes.Clear();
        LaneSwitches.Clear();
    }
}
