using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace RecipeApp.Utils
{
    public class AutoUpdater
    {
        public static UpdateManager UpdateManager = new UpdateManager(new GithubSource(Constants.GitHubRepoUrl, null, false));
        public static UpdateInfo? NewVersion;
        public static bool UpdateAvailable = false;

        public static async Task CheckForUpdatesAsync()
        {
            NewVersion = await UpdateManager.CheckForUpdatesAsync();

            if (NewVersion != null)
            {
                UpdateAvailable = true;
            }
        }

        public static async Task DownloadUpdateAsync()
        {
            if (UpdateAvailable)
            {
                await UpdateManager.DownloadUpdatesAsync(NewVersion!);
            }

            return;
        }

        public static void UpdateAndRestartApp()
        {
            if (UpdateAvailable)
            {
                UpdateManager.ApplyUpdatesAndRestart(NewVersion!);
            }

            return;
        }
    }
}
