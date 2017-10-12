using Restarter.Core.Helpers;
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
            Console.WriteLine(text);
            UpdateManager.CheckForUpdates((wasUpdated) =>
            {
                Thread.Sleep(2000);
                if (wasUpdated)
                    Console.WriteLine("Oxide Updated - Starting Server");
                else
                    Console.WriteLine("Oxide Up To Date - Starting Server");
                StartServer();
            });
        }
        public static void StopServer()
        {
            if (Server == null) return;
            if(!Server.HasExited) Server.Kill();
            Server = null;
        }
        public static void StartServer()
        {
            ServerThread = new Thread(() =>
            {
                Server = Process.GetProcessesByName("RustDedicated").FirstOrDefault();
                if (Server == null)
                {
                    Server = new Process();
                    Server.StartInfo.WorkingDirectory = "";
                    Server.StartInfo.FileName = "start.bat";
                    Server.Start();
                    SlackManager.SendSlackMessage("Started", $"The server has successfully started at {DateTime.Now.ToString("h: mm:ss tt")}.", "Information");
                }
                Server.WaitForExit();
                SlackManager.SendSlackMessage("Started", $"The server has shutdown at {DateTime.Now.ToString("h: mm:ss tt")}. Restarting in 5 seconds.", "Information");
                Console.WriteLine("Server Shut Down - Ending task and restarting.");
                Thread.Sleep(3000);
                RestartServer();
            });
            ServerThread.Start();
        }
    }
}
