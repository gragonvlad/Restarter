using Restarter.Core.Helpers;
using System;
using System.Reflection;

namespace ServerRestarter
{
    public static class Restarter
    {
        static void Main(string[] args)
        {
            ServerManager.RestartServer(true);
            Console.ReadLine();
        }
    }
}
