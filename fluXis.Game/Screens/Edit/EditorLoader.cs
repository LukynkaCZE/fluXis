using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Map;
using fluXis.Game.Online.Activity;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit;

public partial class EditorLoader : FluXisScreen
{
    public override bool AllowMusicControl => false;
    public override bool PlayBackSound => false;
    public override UserActivity InitialActivity => new UserActivity.Editing();

    [Resolved]
    private MapStore maps { get; set; }

    private RealmMap map { get; set; }
    private MapInfo mapInfo { get; set; }

    public EditorLoader(RealmMap realmMap = null, MapInfo map = null)
    {
        this.map = realmMap;
        mapInfo = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new LoadingIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
    }

    public void CreateNewDifficulty(RealmMap realmMap, MapInfo refInfo, string diffName = "new diff", bool copy = false)
    {
        ValidForResume = true;
        this.MakeCurrent();

        var set = maps.GetFromGuid(realmMap.MapSet.ID);
        map = copy ? maps.CopyToNewDifficulty(set, realmMap, refInfo, diffName) : maps.CreateNewDifficulty(set, realmMap, diffName, refInfo);
        mapInfo = map.GetMapInfo();

        pushEditor();
    }

    public void SwitchTo(RealmMap realmMap)
    {
        ValidForResume = true;
        this.MakeCurrent();

        map = realmMap;
        mapInfo = map.GetMapInfo();

        pushEditor();
    }

    private void pushEditor()
    {
        LoadComponentAsync(new Editor(this, map, mapInfo), this.Push);
        ValidForResume = false;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        pushEditor();
    }
}
