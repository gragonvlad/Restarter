using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restarter.Core.Helpers
{
    public static class ProgressManager
    {
        private static double Percentage = 0;
        private static bool IsRunning = false;
        private static Action action;

        public static void UpdateBar(double Percent)
        {
            Percentage = Percent;
        } 
        public static void StopBar()
        {
            IsRunning = false;
            action.Invoke();
        }
        public static bool IsProgressing() => IsRunning;
        public static void CreateBar(string Text, Action callback)
        {
            action = callback;
            IsRunning = true;
            Console.Write(Text);
            using (ProgressBar Bar = new ProgressBar())
            {
                while (IsRunning)
                {
                    Bar.Report(Percentage);
                    Thread.Sleep(20);
                }
            }
        }
    }
}
