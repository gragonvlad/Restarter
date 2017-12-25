using Restarter.Core.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Restarter.Core.Helpers
{
    public static class CPUHelper
    {
        public static List<float> values;
        private static PerformanceCounter cpuCounter;
        private static Timer timer;

        public static void StartCounting()
        {
            if(timer != null)
            {
                if (timer.Enabled) timer.Stop();
                timer = null;
            }
            values = new List<float>();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            timer = new Timer();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            cpuCounter.NextValue();
        }

        public static CPUInformation GetAverage()
        {
            if (timer == null) return null;
            timer.Stop();
            float total = 0;
            float average = 0;
            foreach (var e in values) total += e;
            average = total / values.Count();
            return new CPUInformation()
            {
                averageOverTime = average,
                secondsPassed = values.Count()
            };
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            float value = cpuCounter.NextValue();
            values.Add(value);
        }
    }
}
