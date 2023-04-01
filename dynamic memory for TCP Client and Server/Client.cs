using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Net.NetworkInformation;

namespace dynamic_memory_for_TCP_Client_and_Server
{
    internal class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("TCP dynamic buffer size for server and client side");
        }
        public static void clientside()
        {
            string Host = Dns.GetHostName();
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send("IPAddress"); //Type IP Address here
            string rdip = reply.Address.ToString();
            string IPAddress = rdip;
            int Port = 1234;
            string Filename = "Your filename or file path"; 
            byte[] buffer = null;
            byte[] header = null;
            FileStream fs = new FileStream(Filename, FileMode.Open);
            bool read = true;
            long fileSize = fs.Length;
            int bufferSize = 8192; // 設定初始緩衝區大小
            if (fileSize > 1024 * 1024 * 5) // 如果檔案大於 10 MB，則增加緩衝區大小
            {
                int fileSizeMB = (int)(fileSize / (1024 * 1024));
                bufferSize = fileSizeMB * 2048;
            }
            int bufferCount = Convert.ToInt32(Math.Ceiling((double)fs.Length / (double)bufferSize));
            TcpClient tcpClient = new TcpClient(IPAddress, Port);
            tcpClient.SendTimeout = 600000;
            tcpClient.ReceiveTimeout = 600000;
            string headerStr = "Content-length:" + fs.Length.ToString() + "\r\nFilename:" + $"{Filename}\r\n";
            header = new byte[bufferSize];
            Array.Copy(Encoding.Unicode.GetBytes(headerStr), header, Encoding.Unicode.GetBytes(headerStr).Length);

            tcpClient.Client.Send(header);

            for (int i = 0; i < bufferCount; i++)
            {
                buffer = new byte[bufferSize];
                int size = fs.Read(buffer, 0, bufferSize);

                tcpClient.Client.Send(buffer, size, SocketFlags.Partial);

            }
            Console.WriteLine("Send hwid sccessfully, now delete socket");
            tcpClient.Client.Close();

            fs.Close();
            File.Delete(Host);
        }
    }
}
