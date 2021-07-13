using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameHelper.Utils
{
    public class PortDetector
    {
        public IReadOnlyCollection<ushort> GetUdpPorts(string processName)
        {
            if (string.IsNullOrEmpty(processName)) throw new ArgumentNullException(nameof(processName));

            var processes = Process.GetProcessesByName(processName);
            return GetPortsAndProcesses(processes.Single().Id);
        }

        public IReadOnlyCollection<ushort> GetPortsAndProcesses(int processId)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netstat.exe",
                    Arguments = "-a -o -p UDP",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var content = process.StandardOutput.ReadToEnd();

            var result = new List<ushort>();

            var rows = content.Split(Environment.NewLine);
            foreach (var row in rows)
                if (row.Contains("UDP"))
                {
                    var parts = row.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var pId = int.Parse(parts[^1]);
                    if (pId == processId)
                    {
                        var port = ushort.Parse(parts[1].Split(":")[1]);
                        result.Add(port);
                    }
                }

            return result.OrderBy(p => p).ToArray();
        }
    }
}
