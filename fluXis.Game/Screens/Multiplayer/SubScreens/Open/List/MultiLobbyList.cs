using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.API.Requests.Multiplayer;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.List.UI;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;
using fluXis.Shared.API;
using fluXis.Shared.API.Packets.Multiplayer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;

public partial class MultiLobbyList : MultiSubScreen
{
    public override string Title => "Open Match";
    public override string SubTitle => "Lobby List";

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    private LoadingPanel loadingPanel;

    private FillFlowContainer lobbyList;
    private LoadingIcon loadingIcon;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            lobbyList = new FillFlowContainer
            {
                Width = 1320,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(20),
                Direction = FillDirection.Full,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        fluxel.RegisterListener<MultiJoinPacket>(EventType.MultiplayerJoin, onLobbyJoin);

        loadLobbies();
    }

    private async void loadLobbies()
    {
        loadingIcon.FadeIn(200);
        lobbyList.FadeOut(200).OnComplete(_ => lobbyList.Clear());

        var request = new MultiLobbiesRequest();
        await request.PerformAsync(fluxel);

        var lobbies = request.Response;

        foreach (var lobby in lobbies.Data)
            lobbyList.Add(new LobbySlot { Room = lobby, List = this });

        for (var i = 0; i < 12 - lobbies.Data.Count; i++)
            lobbyList.Add(new EmptyLobbySlot());

        loadingIcon.FadeOut(200);
        lobbyList.FadeIn(200);
    }

    public void JoinLobby(MultiplayerRoom room)
    {
        panels.Content = loadingPanel = new LoadingPanel
        {
            Text = "Joining lobby...",
        };

        fluxel.SendPacketAsync(MultiJoinPacket.CreateC2SInitialJoin(room.RoomID, ""));
    }

    private void onLobbyJoin(FluxelReply<MultiJoinPacket> res)
    {
        if (res.Data == null)
            return;

        if (!res.Data.JoinRequest)
            return;

        Schedule(() =>
        {
            if (res.Success)
            {
                loadingPanel?.Hide();
                this.Push(new MultiLobby { Room = (MultiplayerRoom)res.Data.Room });
            }
            else
            {
                loadingPanel?.Hide();
                notifications.SendError("Failed to join lobby", res.Message);
            }
        });
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 1);
        this.MoveToY(-100).MoveToY(0, 400, Easing.OutQuint);
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.MoveToY(-100, 400, Easing.OutQuint);
        return base.OnExiting(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 1);
        base.OnResuming(e);
    }
}
