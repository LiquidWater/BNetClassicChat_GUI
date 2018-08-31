using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using BNetClassicChat_ClientAPI;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Controls;

namespace BNetClassicChat_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PrivateFields
        private BNetClassicChat_Client client = new BNetClassicChat_Client();
        private bool isConnected = false;
        private Dictionary<ulong, string> idToName;
        #endregion

        #region GUIEventHandlers
        public MainWindow()
        {
            InitializeComponent();
            Init_Client();
        }

        private void WhisperButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Add in whisper functionality to GUI
        }

        private void EmoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                return;
            }
            string msg = InputTextBox.Text;
            InputTextBox.Text = "";
            client.SendEmoteAsync(msg);
            ChatScrollBox.Content += "[SELF]: " + msg + "\n";

            Debug.WriteLine("[GUI]Emote Message \"" + msg + "\" from " + e.Source);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                return;
            }
            string msg = InputTextBox.Text;
            InputTextBox.Text = "";
            client.SendMessageAsync(msg);
            ChatScrollBox.Content += "[SELF]: " + msg + "\n";
 
            Debug.WriteLine("[GUI]Message \"" + msg + "\" from " + e.Source);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(APIKeyBox.Password))
            {
                if (!isConnected)
                {
                    Debug.WriteLine("[GUI]Connecting with API Key [" + APIKeyBox.Password + "]");
                    client.APIKey = APIKeyBox.Password;
                    client.ConnectAsync();
                    ConnectButton.IsEnabled = false;
                    ConnectButton.Opacity = 0.5;
                    APIKeyBox.IsEnabled = false;
                    APIKeyBox.Opacity = 0.5;
                }
                else
                {
                    Debug.WriteLine("[GUI]Disconnecting");
                    client.DisconnectAsync();
                    ConnectButton.IsEnabled = false;
                    ConnectButton.Opacity = 0.5;
                }
            }
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                SendButton_Click(sender, e);
        }
        #endregion

        #region PrivateHelpers
        private void Init_Client()
        {
            //TODO: Better data structure to store more user data such as mod status
            idToName = new Dictionary<ulong, string>();
            client.OnChannelJoin += (obj, e) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                {
                    isConnected = true;
                    ChannelNameLabel.Content = "Channel: " + e.ChannelName;
                    ChatScrollBox.Content += "[SYSTEM] Joined channel " + e.ChannelName + "\n";
                    ConnectButton.IsEnabled = true;
                    ConnectButton.Opacity = 1.0;
                    ConnectButton.Content = "Disconnect";
                }));

            };

            client.OnDisconnect += (obj, e) =>
            {
                idToName.Clear();
                Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                {
                    isConnected = false;
                    ChannelNameLabel.Content = "Channel: Not Connected";
                    ChatScrollBox.Content += "[SYSTEM] Disconnected. Error code: " + e.Code + ". Reason: " + e.Reason + "\n";
                    UserScrollBox.Content = "";
                    APIKeyBox.IsEnabled = true;
                    APIKeyBox.Opacity = 1.0;
                    ConnectButton.IsEnabled = true;
                    ConnectButton.Opacity = 1.0;
                    ConnectButton.Content = "Connect";
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

        //From https://stackoverflow.com/questions/25761795/doing-autoscroll-with-scrollviewer-scrolltoend-only-worked-while-debugging-ev
        /// <summary>
        /// If the scrollviewer is at the bottom, keep the bottom in view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scrollviewer_Messages_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            bool AutoScrollToEnd = true;
            if (sv.Tag != null)
            {
                AutoScrollToEnd = (bool)sv.Tag;
            }
            if (e.ExtentHeightChange == 0)// user scroll
            {
                AutoScrollToEnd = sv.ScrollableHeight == sv.VerticalOffset;
            }
            else// content change
            {
                if (AutoScrollToEnd)
                {
                    sv.ScrollToEnd();
                }
            }
            sv.Tag = AutoScrollToEnd;
            return;
        }
        #endregion
    }
}
