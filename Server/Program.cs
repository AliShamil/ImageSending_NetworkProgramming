using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

//var listener = new Socket(AddressFamily.InterNetwork,
//                          SocketType.Dgram,
//                          ProtocolType.Udp);


//var ip = IPAddress.Parse("127.0.0.1");
//var port = 45678;
//var listenerEP = new IPEndPoint(ip, port);

//listener.Bind(listenerEP);

//var buffer = new byte[ushort.MaxValue - 29];
//EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);


//var count = 0;
//var msg = string.Empty;






//while (true)
//{
//    var result = await listener.ReceiveFromAsync(new ArraySegment<byte>(buffer),
//                                                 SocketFlags.None,
//                                                 remoteEP);

//    count = result.ReceivedBytes;
//    msg = Encoding.Default.GetString(buffer, 0, count);
//    Console.WriteLine($"{result.RemoteEndPoint} : {msg}");
//}

class Program
{
    static async Task Main()
    {
        Socket udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, 12345);
        udpServer.Bind(localEndpoint);
        Console.WriteLine($"Server started on {localEndpoint}");
        while (true)
        {
            EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBuffer = new byte[1024];
            var result = await udpServer.ReceiveFromAsync(receiveBuffer, SocketFlags.None, remoteEndpoint) ;
            Console.WriteLine($"Received request from {remoteEndpoint}");

            Bitmap screenshot = GetScreenshot();
            byte[] imageData = ImageToByteArray(screenshot);
            await udpServer.SendToAsync(imageData, SocketFlags.None, remoteEndpoint);
            Console.WriteLine($"Sent screenshot to {remoteEndpoint}");
        }
    }

    static Bitmap GetScreenshot()
    {
        Bitmap bitmap = new Bitmap("C:\\Users\\Admin\\Desktop\\c9dea34d94fd90eb00a809be55c9bb44.jpg");
        return bitmap;
        
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