using System;
using System.IO;
using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.Maintenance;

public partial class MaintenanceFilesSection : SettingsSubSection
{
    public override string Title => "Files";

    [BackgroundDependencyLoader]
    private void load(Storage storage, MapStore store, NotificationManager notifications)
    {
        AddRange(new Drawable[]
        {
            new SettingsButton
            {
                Label = "Clean up files",
                Description = "Deletes all files that are not used by any maps",
                ButtonText = "Run",
                Action = () =>
                {
                    notifications.SendSmallText("Cleaning up files...", FontAwesome.Solid.Sync);
                    var deleted = 0;
                    var errors = 0;

                    foreach (var directory in storage.GetDirectories("maps"))
                    {
                        var guid = directory.Split(Path.DirectorySeparatorChar).Last();

                        if (store.MapSets.All(m => m.ID.ToString() != guid))
                        {
                            try
                            {
                                storage.DeleteDirectory(directory);
                                deleted++;
                            }
                            catch (Exception ex)
                            {
                                errors++;
                                Logger.Error(ex, $"Failed to delete directory {directory}");
                            }
                        }
                    }

                    notifications.SendText($"Cleaned up {deleted} folder(s)", errors != 0 ? $"{errors} deletion(s) failed." : "", FontAwesome.Solid.Check);
                }
            }
        });
    }
}
