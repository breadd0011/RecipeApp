using System.Threading.Tasks;
using Velopack;

namespace RecipeApp.Utils
{
    public class AutoUpdater
    {
        public static UpdateManager UpdateManager = new(Constants.GitHubRepoUrl);
        public static UpdateInfo? Update;
        public static bool UpdateAvailable => Update != null;

        public static async Task CheckForUpdatesAsync()
        {
            Update = await UpdateManager.CheckForUpdatesAsync();
        }

        public static async Task DownloadUpdateAsync()
        {
            if (UpdateAvailable)
            {
                await UpdateManager.DownloadUpdatesAsync(Update!);
            }

            return;
        }

        public static void UpdateAndRestartApp()
        {
            if (UpdateAvailable)
            {
                UpdateManager.ApplyUpdatesAndRestart(Update!);
            }

            return;
        }
    }
}
