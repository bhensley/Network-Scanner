using System;
using System.Collections.Generic;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuMain_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void txtSubnet_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSubnet.Text == "Subnet") txtSubnet.Text = "";
        }

        private void txtSubnet_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSubnet.Text.Length == 0) txtSubnet.Text = "Subnet";
        }
    }
}
