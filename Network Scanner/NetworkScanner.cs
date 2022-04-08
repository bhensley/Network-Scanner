using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Network_Scanner
{
    internal class NetworkScanner
    {
        public string Subnet { get; set; }
        public List<ScanResult> ScanResults = new List<ScanResult>();
        public int resultsFound = 0;

        static object lockObj = new object();

        public event EventHandler<List<ScanResult>> PingEvent;

        public NetworkScanner ()
        { }

        public NetworkScanner (string subnet) {
            // Let's just make sure we're set up with a proper formatted Subnet
            // (x.x.x. rather than x.x.x)
            if (subnet[subnet.Length - 1] != '.')
            {
                Subnet = subnet + ".";
            } else
            {
                Subnet = subnet;
            }
        }

        public async void PerformScanAsync ()
        {
            var tasks = new List<Task>();

            resultsFound = 0;

            for (int i = 1; i < 255; i++)
            {
                string ip = Subnet + i.ToString();

                Ping p = new Ping();
                var task = PingAndAddAsync(p, ip);
                tasks.Add(task);
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (PingException ex)
            {

            }

            PingEvent?.Invoke(this, ScanResults);
        }

        private async Task PingAndAddAsync (Ping p, string ip)
        {
            var reply = await p.SendPingAsync(ip, 250).ConfigureAwait(false);

            if (reply != null && reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                // See if we can snag the hostname
                string hostName;
                try
                {
                    System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(ip);
                    hostName = hostEntry.HostName;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    hostName = "Unknown";
                }

                ScanResults.Add(new ScanResult
                {
                    IpAddress = ip,
                    Hostname = hostName,
                    MacAddress = GetMacAddress(ip),
                });

                lock (ScanResults) {
                    resultsFound++;
                }
            }
        }

        private string GetMacAddress (string ip)
        {
            var pairs = GetMacIpPairs();

            foreach (var pair in pairs)
            {
                if (pair.IpAddress == ip)
                {
                    return pair.MacAddress;
                }
            }

            return null;
        }

        /**
         * Doing that StackOverflow thing... this solution comes from:
         * https://stackoverflow.com/questions/12802888/get-a-machines-mac-address-on-the-local-network-from-its-ip-in-c-sharp
         * No shame!
         * 
         * private string GetMacAddress (string ip)
         */
        private IEnumerable<MacIpPair> GetMacIpPairs()
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();

            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches (cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                yield return new MacIpPair()
                {
                    MacAddress = m.Groups["mac"].Value,
                    IpAddress = m.Groups["ip"].Value
                };
            }
        }
    }
}
