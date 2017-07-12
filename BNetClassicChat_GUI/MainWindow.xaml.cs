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
        public MainWindow()
        {
            InitializeComponent();
            ChatScrollBox.Content = "Test\n";
            UserScrollBox.Content = "Test2\n";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.Source);
            ChatScrollBox.Content += "test\n";
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.Source);
            UserScrollBox.Content += "test2\n";
        }
    }
}
