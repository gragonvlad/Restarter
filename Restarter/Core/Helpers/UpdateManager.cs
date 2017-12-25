using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Ionic.Zip;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using ServerRestarter;
using Restarter.Core.Objects;

namespace Restarter.Core.Helpers
{
    public static class UpdateManager
    {
        private static void DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ProgressManager.StopBar();
        }

        private static void DownloadChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            ProgressManager.UpdateBar(bytesIn / totalBytes * 100);
        }
        public static UpdateInformation GetInformation()
        {
            if (!File.Exists("RustDedicated_Data/Managed/Oxide.Rust.dll"))
            {
                return new UpdateInformation()
                {
                    currentVersion = "Oxide Not Found",
                    latestVersion = "Oxide Not Found",
                    needsUpdate = true
                };
            }
            AssemblyName name = AssemblyName.GetAssemblyName("RustDedicated_Data/Managed/Oxide.Rust.dll");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "OxideMod");

                using (var response = client.GetAsync("https://api.github.com/repos/OxideMod/Oxide/releases/latest").Result)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    GithubLatest latest = JsonConvert.DeserializeObject<GithubLatest>(json);

                    string s = latest.html_url;
                    s = Regex.Replace(s, "[^0-9.]", "").Remove(0, 1);

                    return new UpdateInformation()
                    {
                        currentVersion = name.Version.ToString(),
                        latestVersion = s,
                        needsUpdate = !name.Version.ToString().Contains(s)
                    };
                }
            }
        }
        public static void CheckForUpdates(Action<bool> callback)
        {
            if (!Directory.Exists("Restarter")) Directory.CreateDirectory("Restarter");
            UpdateInformation information = GetInformation();
            if (!information.needsUpdate)
            {
                Logger.LogInfo($"Oxide is up to date.\n * Found Version: {information.currentVersion}\n * Latest Version: {information.latestVersion}");
                callback.Invoke(false);
                return;
            }
            Logger.LogWarning($"Oxide is out of date.\n * Found Version: {information.currentVersion}\n * Latest Version: {information.latestVersion}");
            SlackManager.SendSlackMessage("Check", $"Oxide is out of date.\n * Found Version: {information.currentVersion}\n * Latest Version: {information.latestVersion}", "Updating");
            Thread thread = new Thread(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += DownloadChanged;
                    wc.DownloadFileCompleted += DownloadCompleted;
                    wc.DownloadFileAsync(new Uri("https://github.com/OxideMod/Oxide/releases/download/latest/Oxide-Rust.zip"), "Restarter/OxideRecent.zip");
                }
            });
            thread.Start();
            ProgressManager.CreateBar("Downloading Oxide", () => {
                Logger.LogInfo("\nDownload Completed");
                thread.Abort();
                thread = new Thread(() =>
                {
                    using (ZipFile zip = ZipFile.Read("Restarter/OxideRecent.zip"))
                    {
                        int run = 0;
                        foreach (ZipEntry e in zip)
                        {
                            if (e.FileName.Contains("start-example")) continue;
                            run++;
                            ProgressManager.UpdateBar(zip.EntryFileNames.Count() / run * 100);
                            e.Extract(Directory.GetCurrentDirectory(), ExtractExistingFileAction.OverwriteSilently);
                        }
                        ProgressManager.StopBar();
                    }
                });
                thread.Start();
                ProgressManager.CreateBar("Extracting Files", () =>
                {
                    Logger.LogInfo("\nUpdate completed.Starting server.");
                    callback.Invoke(true);
                    thread.Abort();
                });
            });
        }
    }
}
