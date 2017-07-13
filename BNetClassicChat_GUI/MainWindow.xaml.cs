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
using BNetClassicChat_ClientAPI;
using System.Diagnostics;

namespace BNetClassicChat_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BNetClassicChat_Client client;
        private bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = InputTextBox.Text;
            InputTextBox.Text = "";
            Debug.WriteLine("Message " + msg + " from " + e.Source);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.Source);

            if (!isConnected)
            {
                Debug.WriteLine("Connecting with API Key [" + APIKeyBox.Password + "]");
                Init_Client();
                ConnectButton.Content = "Disconect";
                isConnected = true;
            }
            else
            {
                Debug.WriteLine("Disconnecting");
                Dispose_Client();
                ConnectButton.Content = "Connect";
                isConnected = false;
            }

        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                SendButton_Click(sender, e);
        }

        private void Init_Client()
        {
            client = new BNetClassicChat_Client(APIKeyBox.Password);
        }

        private void Dispose_Client()
        {

        }
    }
}
