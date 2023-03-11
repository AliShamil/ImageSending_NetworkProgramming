using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;


class Program
{
    static async Task Main()
    {
        Socket udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
        udpServer.Bind(localEndpoint);
        Console.WriteLine($"Server started on {localEndpoint}");
        
        while (true)
        {
            EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBuffer = new byte[ushort.MaxValue - 29];
            var result = await udpServer.ReceiveFromAsync(receiveBuffer, SocketFlags.None, remoteEndpoint);
            Console.WriteLine($"Received request from {result.RemoteEndPoint}");

            Bitmap screenshot = GetScreenshot();
            byte[] imageData = ImageToByteArray(screenshot);


            Console.WriteLine(imageData.Length);

            var chunk = imageData.Chunk(ushort.MaxValue - 29);

            var newBuffer = chunk.ToArray();

            for (int i = 0; i < newBuffer.Length; i++)
            {
                await Task.Delay(50);
                await udpServer.SendToAsync(newBuffer[i], SocketFlags.None, result.RemoteEndPoint);
            }

            Console.WriteLine($"Sent screenshot to {result.RemoteEndPoint}");
        }
    }

    static Bitmap GetScreenshot()
    {
        Bitmap memoryImage;
        memoryImage = new Bitmap(1920, 1080);

        Graphics memoryGraphics = Graphics.FromImage(memoryImage);
        memoryGraphics.CopyFromScreen(0, 0, 0, 0, memoryImage.Size);

        return memoryImage;

    }

    static byte[] ImageToByteArray(Image image)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            image.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}