using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Network_Scanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkScanner networkScanner = new NetworkScanner();
        string defaultSubnetText = "Subnet";

        public MainWindow()
        {
            InitializeComponent();

            // Register the network scanner ping event, so we can update the datagrid
            networkScanner.PingEvent += NetworkScanner_PingEvent;

            // Prefill the subnet with the current, local subnet
            txtSubnet.Text = GetCurrentSubnet();
        }

        private string GetCurrentSubnet()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            // Only work with IPv4 for right now
            // TODO: Integrated IPv6?
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return String.Join(".", ip.GetAddressBytes()[0..3]) + ".";
                }
            }

            return defaultSubnetText;
        }

        private void menuMain_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void txtSubnet_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSubnet.Text == defaultSubnetText) txtSubnet.Text = "";
        }

        private void txtSubnet_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSubnet.Text.Length == 0) txtSubnet.Text = defaultSubnetText;
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            string subnet = txtSubnet.Text;
            if (subnet[subnet.Length - 1] != '.') subnet += ".";

            networkScanner.Subnet = subnet;
            networkScanner.PerformScanAsync();
        }
        private void NetworkScanner_PingEvent(object sender, List<ScanResult> e)
        {
            dgScanResults.ItemsSource = e;
        }

    }
}
