using Restarter.Core.Helpers;
using Restarter.Core.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restarter.Core.Helpers
{
    public static class ServerManager
    {
        private static Process Server;
        private static Thread ServerThread;
        
        public static void RestartServer(bool firstStart = false)
        {
            StopServer();
            var text = (firstStart) ? "Server Startup Initiated - Checking for updates" : "Server Shutdown - Checking for updates";
            Logger.LogInfo(text);
            UpdateManager.CheckForUpdates((wasUpdated) =>
            {
                Thread.Sleep(2000);
                if (wasUpdated)
                    Logger.LogInfo("Oxide Updated - Starting Server");
                else
                    Logger.LogInfo("Oxide Already Up To Date - Starting Server");
                StartServer();
            });
        }
        public static void StopServer()
        {
            CPUInformation info = CPUHelper.GetAverage();
            if(info != null) Logger.LogInfo($"Average CPU Usage over {info.secondsPassed}s. %{info.averageOverTime}");
            if (Server == null) return;
            if(!Server.HasExited) Server.Kill();
            Server = null;
        }
        public static void StartServer()
        {
            IniFile file = new IniFile("restarter.ini");
            ServerThread = new Thread(() =>
            {
                Server = Process.GetProcessesByName("RustDedicated").FirstOrDefault();
                if (Server == null)
                {
                    Server = new Process();
                    Server.StartInfo.WorkingDirectory = file.Read("ServerPath", "ServerInformation");
                    Server.StartInfo.FileName = file.Read("BatchFile", "ServerInformation");
                    Server.Start();
                    CPUHelper.StartCounting();
                    SlackManager.SendSlackMessage("Started", $"The server has successfully started at {DateTime.Now.ToString("h: mm:ss tt")}.", "Information");
                    Logger.LogInfo("Server Started Successfully.");
                }
                Server.WaitForExit();
                SlackManager.SendSlackMessage("Shutdown", $"The server has shutdown at {DateTime.Now.ToString("h: mm:ss tt")}. Restarting.", "Information");
                Logger.LogWarning("Server Shut Down - Ending task and restarting.");
                RestartServer();
            });
            ServerThread.Start();
        }
    }
}
