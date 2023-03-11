using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using System.IO;

namespace Client
{
    public partial class MainWindow : Window
    {

        private Socket udpClient;
        private IPEndPoint serverEndpoint;

        public MainWindow()
        {
            InitializeComponent();
            udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
        }

        private async void ReceiveScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                udpClient.SendTo(new byte[] { 1 }, serverEndpoint);
                EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receiveBuffer = new byte[1024];
                var result = await udpClient.ReceiveFromAsync(receiveBuffer, SocketFlags.None, remoteEndpoint);
                ImageBox.Source = ByteArrayToImage(receiveBuffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error receiving screenshot: {ex.Message}");
            }
        }

        private BitmapImage ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
        }
    }
    }

