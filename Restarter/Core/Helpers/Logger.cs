using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restarter.Core.Helpers
{
    public static class Logger
    {
        public static void LogInfo(object log)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(log.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void LogWarning(object log)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(log.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void LogError(object log)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
