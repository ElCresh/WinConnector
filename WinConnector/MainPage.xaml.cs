using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WinConnector
{
    public sealed partial class MainPage : Page
    {
        private Connector connector;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!IsConnected())
            {
                btnConnect.IsEnabled = false;
                string serverToConnect = txtRemoteServer.Text;

                AddLogLine("Trying to connect to " + serverToConnect);

                Uri uriServerToConnect = new Uri(serverToConnect);
                switch (uriServerToConnect.Scheme)
                {
                    case "ftp":
                        bool res = OpenFtpConnection(uriServerToConnect);

                        if (res)
                        {
                            AddLogLine("Connected with success");
                            UpdateDirectoryList();
                            btnDisconnect.IsEnabled = true;
                        }
                        else
                        {
                            AddLogLine("Unable to connect");
                            btnConnect.IsEnabled = true;
                        }
                        break;
                    default:
                        AddLogLine("Unknown protocol");
                        btnConnect.IsEnabled = true;
                        break;
                }
            }
            else
            {
                AddLogLine("Connection already open");
            }
        }

        private bool OpenFtpConnection(Uri uri)
        {
            connector = new FtpConnector();
            return connector.Connect(uri);
        }

        private bool IsConnected()
        {
            return connector != null && connector.IsConnected();
        }

        private void AddLogLine(string line)
        {
            listDebug.Items.Add(line);
        }

        private void UpdateDirectoryList()
        {
            listDirecories.Items.Clear();
            ArrayList direcories = connector.GetListOfCurrentDirectory();
            foreach(string directory in direcories)
            {
                listDirecories.Items.Add(directory);
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            connector.Disconnect();
            UpdateDirectoryList();
            btnConnect.IsEnabled = true;
            btnDisconnect.IsEnabled = false;
            btnDownload.IsEnabled = false;
            AddLogLine("Disconnected from server");
        }

        private void listDirecories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDownload.IsEnabled = true;
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = (string) listDirecories.SelectedItem;
            AddLogLine("Trying to download: " + selectedFile);
            bool res = await connector.Download(selectedFile);
            if (res)
            {
                AddLogLine("Download completed");
            }
            else
            {
                AddLogLine("Download failed");
            }
        }
    }
}
