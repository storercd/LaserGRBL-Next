using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LaserGRBL
{
    public static class Application2
    {
        public static void RestartNoCommandLine()
        {
            ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
            // In .NET 8+, use Environment.ProcessPath to get the actual executable
            startInfo.FileName = Environment.ProcessPath ?? Application.ExecutablePath;
            var exit = typeof(Application).GetMethod("ExitInternal",
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Static);
            exit.Invoke(null, null);
            Process.Start(startInfo);
        }
    }
}
