using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using BNetClassicChat_ClientAPI;
using System.Diagnostics;
using System.Windows.Threading;

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

        private void WhisperButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Add in whisper functionality to GUI
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
            ChatScrollBox.Content += "[SELF]: " + msg + "\n";
 
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

        #region PrivateHelpers
        private bool Init_Client()
        {
            //TODO: Better data structure to store more user data such as mod status
            idToName = new Dictionary<ulong, string>();
            client = new BNetClassicChat_Client(APIKeyBox.Password);
            client.OnChannelJoin += (obj, e) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                {
                    ChannelNameLabel.Content = "Channel: " + e.ChannelName;
                    ChatScrollBox.Content += "[SYSTEM] Joined channel " + e.ChannelName + "\n";
                }));
                
            };

            client.OnChatMessage += (obj, e) =>
            {
                string username = "";
                try
                {
                    username = idToName[e.UserId];
                    Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                    {
                        ChatScrollBox.Content += username + ": " + e.Message + "\n";
                    }));
                }
                catch (Exception)
                {
                    Debug.WriteLine("[GUI]Userid [" + e.UserId + "] not found");
                }
            };

            client.OnUserJoin += (obj, e) =>
            {
                try
                {
                    idToName.Add(e.UserId, e.ToonName);
                    Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                    {
                        Update_User_View();
                        ChatScrollBox.Content += "[SYSTEM]" + e.ToonName + " has joined.\n";
                    }));
                }
                catch (Exception)
                {
                    Debug.WriteLine("[GUI]Userid [" + e.UserId + "] already exists");
                }
            };

            client.OnUserLeave += (obj, e) =>
            {
                try
                {
                    string tempusername = idToName[e.UserId];
                    idToName.Remove(e.UserId);
                    Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                    {
                        Update_User_View();
                        ChatScrollBox.Content += "[SYSTEM]" + tempusername + " has left.\n";
                    }));
                }
                catch (Exception)
                {
                    Debug.WriteLine("[GUI]Userid [" + e.UserId + "] cannot be deleted");
                }
            };

            client.Connect();

            //TODO: some sort of way to figure out when the connection fails
            return true;
        }

        //These methods to be called within a GUI dispatcher thread
        private void Dispose_Client()
        {
            client.Disconnect();
            UserScrollBox.Content = "";
            ChatScrollBox.Content += "Disconnected!";
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
