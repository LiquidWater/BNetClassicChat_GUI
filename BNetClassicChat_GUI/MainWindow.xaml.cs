using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using BNetClassicChat_ClientAPI;
using System.Diagnostics;

namespace BNetClassicChat_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PrivateFields
        private BNetClassicChat_Client client;
        private bool isConnected = false;
        private Dictionary<ulong, string> idToName;
        #endregion

        #region GUIEventHandlers
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                return;
            }
            string msg = InputTextBox.Text;
            InputTextBox.Text = "";
            client.SendMessage(msg);

            this.Dispatcher.Invoke(() =>
            {
                ChatScrollBox.Content += "[SELF]: " + msg + "\n";
            });

            Debug.WriteLine("[GUI]Message \"" + msg + "\" from " + e.Source);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.Source);

            if (!isConnected)
            {
                Debug.WriteLine("[GUI]Connecting with API Key [" + APIKeyBox.Password + "]");
                if (Init_Client())
                {
                    ConnectButton.Content = "Disconnect";
                    isConnected = true;
                }
            }
            else
            {
                Debug.WriteLine("[GUI]Disconnecting");
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
        #endregion

        #region Private Helpers
        private bool Init_Client()
        {
            idToName = new Dictionary<ulong, string>();
            client = new BNetClassicChat_Client(APIKeyBox.Password);
            client.OnChannelJoin += (obj, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    ChannelNameLabel.Content = "Channel: " + e.ChannelName;
                    ChatScrollBox.Content += "[SYSTEM] Joined channel " + e.ChannelName + "\n";
                });
                
            };

            client.OnChatMessage += (obj, e) =>
            {
                string username = "";
                try
                {
                    username = idToName[e.UserId];
                }
                catch (Exception)
                {

                }

                this.Dispatcher.Invoke(() => 
                {
                    ChatScrollBox.Content += username + ": " + e.Message + "\n";
                });
                
            };

            client.OnUserJoin += (obj, e) =>
            {
                try
                {
                    idToName.Add(e.UserId, e.ToonName);
                }
                catch (Exception)
                {

                }

                this.Dispatcher.Invoke(() =>
                {
                    Update_User_View();
                    ChatScrollBox.Content += "[SYSTEM]" + e.ToonName + " has joined.\n";
                });
            };

            client.OnUserLeave += (obj, e) =>
            {
                try
                {
                    idToName.Remove(e.UserId);
                    this.Dispatcher.Invoke(() =>
                    {
                        Update_User_View();
                        ChatScrollBox.Content += "[SYSTEM]" + idToName[e.UserId] + " has left.\n";
                    });
                }
                catch (Exception)
                {

                }
            };

            client.Connect();

            //TODO: some sort of way to figure out when the connection fails
            return true;
        }

        private void Dispose_Client()
        {
            client.Disconnect();
            UserScrollBox.Content = "";
            ChannelNameLabel.Content = "Channel: Not Connected";
        }

        //Updates the scrollbar that contains the list of users in channel
        private void Update_User_View()
        {
            UserScrollBox.Content = "";
            foreach (KeyValuePair<ulong, string> k in idToName)
            {
                UserScrollBox.Content += k.Value + "\n";
            }
        }
        #endregion
    }
}
