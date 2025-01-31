using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.BottomBar.Timeline;

public partial class EditorTimeline : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    private TimelineIndicator indicator;
    private Container chorusPoints;
    private Container timingPoints;
    private TimelineDensity densityContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            new Circle
            {
                RelativeSizeAxes = Axes.X,
                Height = 5,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            chorusPoints = new Container
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Height = 8,
                Y = -20
            },
            timingPoints = new Container
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Height = 8,
                Y = -8
            },
            densityContainer = new TimelineDensity(),
            indicator = new TimelineIndicator()
        };

        foreach (var timingPoint in values.Editor.MapInfo.TimingPoints)
        {
            var x = timingPoint.Time == 0 ? 0 : timingPoint.Time / clock.TrackLength;
            if (!double.IsFinite(x) || double.IsNaN(x)) x = 0;

            timingPoints.Add(new Triangle
            {
                RelativePositionAxes = Axes.X,
                Size = new Vector2(8),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Rotation = 180,
                X = x
            });
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition);
        return true;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        seekToMousePosition(e.MousePosition);
        return true;
    }

    protected override void OnDrag(DragEvent e) => seekToMousePosition(e.MousePosition);

    private void seekToMousePosition(Vector2 position)
    {
        // why is there a 20px offset??
        float x = Math.Clamp(position.X - 20, 0, DrawWidth);
        float progress = x / DrawWidth;
        clock.SeekSmoothly(progress * clock.TrackLength);
    }

    protected override void Update()
    {
        var x = clock.CurrentTime / clock.TrackLength;
        if (!double.IsFinite(x) || double.IsNaN(x)) x = 0;

        indicator.X = (float)x;
    }
}
