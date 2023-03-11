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
            var receivedBuffer = new byte[ushort.MaxValue - 29];
            udpClient.SendTo(receivedBuffer, serverEndpoint);
            
            var list = new List<byte>();
            var len = 0;
            var totalBytes = 0;

            do
            {
                var result = await udpClient.ReceiveFromAsync(receivedBuffer, SocketFlags.None, serverEndpoint);
                len = result.ReceivedBytes;
                list.AddRange(receivedBuffer.Take(len));
                totalBytes += len;

            } while (len == receivedBuffer.Length);

            try
            {
                var image = ByteArrayToImage(list.ToArray());
                ImageBox.Source = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private BitmapImage ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(byteArray))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}

