using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace dynamic_memory_for_TCP_Client_and_Server
{
    internal class Server
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("TCP dynamic buffer size for server and client side");
        }

        public static void serverside()
        {

            int Port = 1234;
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (true)
            {
                Socket socket = listener.AcceptSocket();
                string clientIPAddress = $"{DateTime.Now}: Connected Client IP is: " + IPAddress.Parse(((IPEndPoint)socket.RemoteEndPoint).Address.ToString());
                Console.WriteLine(clientIPAddress);
                byte[] header = new byte[2048];
                socket.Receive(header);
                string headerStr = Encoding.Unicode.GetString(header);
                string[] splitted = headerStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach (string s in splitted)
                {
                    if (s.Contains(":"))
                    {
                        headers.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1));
                    }
                }

                if (headers.ContainsKey("Content-length") && headers.ContainsKey("Filename"))
                {
                    int filesize = Convert.ToInt32(headers["Content-length"]);
                    string filename = headers["Filename"];
                    try
                    {
                        FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
                        int bufferSize = 2048;
                        int bufferCount = Convert.ToInt32(Math.Ceiling((double)filesize / (double)bufferSize));
                        byte[] buffer = new byte[bufferSize];
                        int bytesRead = 0;
                        int totalBytesRead = 0;
                        while (totalBytesRead < filesize)
                        {
                            bytesRead = socket.Receive(buffer, Math.Min(bufferSize, filesize - totalBytesRead), SocketFlags.None);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            fs.Write(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                        }
                        Console.WriteLine($"{DateTime.Now} Total received: {totalBytesRead},successfully received from Client {((IPEndPoint)socket.RemoteEndPoint).Address}");
                        fs.Close();
                        socket.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {

                    }
                }
                else
                {
                    Console.WriteLine("Invalid header format");
                    socket.Close();
                }
            }
        }
    }
}
