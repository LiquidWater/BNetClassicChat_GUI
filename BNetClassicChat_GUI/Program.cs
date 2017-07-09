using BNetClassicChat_ClientAPI;
using System;
using System.IO;
using System.Windows.Forms;

namespace BNetClassicChat_GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());

            string apiKey = File.ReadAllLines("Config/APIKey.txt")[0];

            BNetClassicChat_Client api = new BNetClassicChat_Client(apiKey);
        }
    }
}
