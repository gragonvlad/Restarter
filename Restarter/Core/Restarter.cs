using Restarter.Core.Helpers;
using System;
using Microsoft.Win32;
using System.Reflection;
using System.IO;

namespace ServerRestarter
{
    public static class Restarter
    {
        static void Main(string[] args)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (key.GetValue("RustRestarter") == null)
                key.SetValue("RustRestarter", "\"" + Assembly.GetExecutingAssembly().Location + "\"");

            IniFile file = new IniFile("restarter.ini");
            if(!file.KeyExists("ServerPath", "ServerInformation"))
                file.Write("ServerPath", @"C:\YourServer\", "ServerInformation");
            if(!file.KeyExists("BatchFile", "ServerInformation"))
                file.Write("BatchFile", "replace_me.bat", "ServerInformation");
            if (!file.KeyExists("UseSlack", "SlackInformation"))
                file.Write("UseSlack", "true", "SlackInformation");

            if(file.Read("BatchFile", "ServerInformation") == "replace_me.bat")
            {
                Logger.LogError($"Please edit your restarter.ini at {Directory.GetCurrentDirectory()}");
                Console.Read();
                return;
            }
            if(!File.Exists($@"{file.Read("ServerPath", "ServerInformation")}\{file.Read("BatchFile", "ServerInformation")}"))
            {
                Logger.LogError($@"There is no bat file at: {file.Read("ServerPath", "ServerInformation")}\{file.Read("BatchFile", "ServerInformation")}");
                Console.Read();
                return;
            }

            ServerManager.RestartServer(true);
            Console.ReadLine();
        }
    }
}
