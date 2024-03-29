﻿using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

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

            if(imageData.Length / 1024f>=1000)
                Console.WriteLine($"{(imageData.Length / 1024f)/1024f} mb");
            else 
                Console.WriteLine($"{imageData.Length / 1024f} kb");


            var chunk = imageData.Chunk(ushort.MaxValue - 29);

            var buffer = chunk.ToArray();

            for (int i = 0; i < buffer.Length; i++)
            {
                await Task.Delay(30);
                await udpServer.SendToAsync(buffer[i], SocketFlags.None, result.RemoteEndPoint);
            }

            Console.WriteLine($"Sent screenshot to {result.RemoteEndPoint}");
            Console.WriteLine();

        }
    }

    static Bitmap GetScreenshot()
    {
        Bitmap screenshot;
        screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, 
            Screen.PrimaryScreen.Bounds.Height);

        Graphics graphics = Graphics.FromImage(screenshot);
        graphics.CopyFromScreen(0, 0, 0, 0, screenshot.Size);

        return screenshot;
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