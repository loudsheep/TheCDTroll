using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Text.RegularExpressions;

namespace TheCDTrollGUI
{
    public class Connection
    {
        public static IPAddress[] GetLocalAddreses()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> list = new List<IPAddress>();
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                //if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork && IsIPv4Address(host.AddressList[i].ToString()))
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    list.Add(host.AddressList[i]);
                }
            }

            return list.ToArray();
        }

        public static bool IsIPv4Address(string address)
        {
            Regex check = new Regex("\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\\.|$)){4}\b");
            return check.Matches(address).Count > 0;
            //return check.IsMatch(address);
        }

        public static void ListenForCommands()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = GetLocalAddreses()[1];

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public static void ListenForCommandsWithCallback(Func<string, string> func)
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;

                IPAddress[] addresses = GetLocalAddreses();

                Console.WriteLine("listening on " + addresses[0] + ":" + port);
                IPAddress localAddr = addresses[0];

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        func(data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        public static void ListenForCommandsWithCallbackOnAllLocalAddresses(Func<string, string> func)
        {
            var ips = GetLocalAddreses();

            Console.WriteLine("Listening on all local IPs");
            List<Thread> threads = new List<Thread>();
            foreach (var ip in ips)
            {
                //SendDirectMessage(message, ip, port);
                Thread thread = new Thread(ListenOnAddress);
                //thread.Start(new ListenData() { ip = ip, func = func});
                threads.Add(thread);
            }

            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();
            }
        }

        public class ListenData
        {
            public IPAddress ip;
            public Func<string, int> func;
        }

        public static void ListenOnAddress(object listenData)
        {
            if(listenData is ListenData)
            {
                ListenData ls = (ListenData)listenData;

                TcpListener server = null;
                UdpClient udp = new UdpClient();
                try
                {
                    int port = 13000;

                    Console.WriteLine("listening on " + ls.ip + ":" + port);
                    IPAddress localAddr = ls.ip;
                    server = new TcpListener(localAddr, port);
                    server.Start();

                    Byte[] bytes = new Byte[256];
                    String data = null;

                    while (true)
                    {
                        Console.Write("Waiting for a connection... ");
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");
                        data = null;
                        NetworkStream stream = client.GetStream();

                        int i;
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, i);
                            ls.func(data);
                        }

                        client.Close();
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
            }
        }

        public static void ListenOnAddressUDP(object listenData)
        {
            if (listenData is ListenData)
            {
                ListenData ld = (ListenData)listenData;
                UdpClient client = new UdpClient(13000);
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    try
                    {
                        // Blocks until a message returns on this socket from a remote host.
                        Byte[] receiveBytes = client.Receive(ref iPEndPoint);
                        Console.WriteLine(client.ToString());

                        string returnData = Encoding.ASCII.GetString(receiveBytes);

                        ld.func(returnData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            //TcpListener server = null;
            //UdpClient udp = new UdpClient(;
            //try
            //{
            //    int port = 13000;

            //    Console.WriteLine("listening on " + ls.ip + ":" + port);
            //    IPAddress localAddr = ls.ip;
            //    server = new TcpListener(localAddr, port);
            //    server.Start();

            //    Byte[] bytes = new Byte[256];
            //    String data = null;

            //    while (true)
            //    {
            //        Console.Write("Waiting for a connection... ");
            //        TcpClient client = server.AcceptTcpClient();
            //        Console.WriteLine("Connected!");
            //        data = null;
            //        NetworkStream stream = client.GetStream();

            //        int i;
            //        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            //        {
            //            data = Encoding.ASCII.GetString(bytes, 0, i);
            //            ls.func(data);
            //        }

            //        client.Close();
            //    }
            //}
            //catch (SocketException e)
            //{
            //    Console.WriteLine("SocketException: {0}", e);
            //}
        }

        private static int LastMask(int mask)
        {
            int idx = mask / 8;
            mask -= idx * 8;
            int last = 0;
            for (int i = 7; i >= 0 && mask > 0; i--, mask--)
            {
                last += (int)Math.Pow(2, i);
            }
            return last;
        }

        public static IPAddress[] GetAddressesInNetwork(string net_ip)
        {
            List<IPAddress> addresses = new List<IPAddress>();

            string[] addrParts = net_ip.Split('/');
            if (addrParts.Length != 2) throw new Exception("Incorrect network address fromat, example format: 192.168.1.1/24");

            int netMask;
            if (!int.TryParse(addrParts[1], out netMask)) throw new Exception("Incorrect network mask fromat, example format: 192.168.1.1/24");
            if (netMask < 8 || netMask > 30) throw new Exception("Network mask can't be below 8 and above 30");

            int[] netAddr = new int[4];
            string[] netAddrPart = addrParts[0].Split('.');
            if (netAddrPart.Length != 4) throw new Exception("Incorrect network address fromat, example format: 192.168.1.1/24");
            for (int i = 0; i < 4; i++)
            {
                int tmp;
                if (!int.TryParse(netAddrPart[i], out tmp)) throw new Exception("Incorrect network address fromat, example format: 192.168.1.1/24");
                netAddr[i] = tmp;
            }
            int lastMask = LastMask(netMask);
            netAddr[netMask / 8] &= lastMask;
            for (int i = netMask / 8 + 1; i < 4; i++)
            {
                netAddr[i] = 0;
            }

            for (int i = 1; i < Math.Pow(2, 32 - netMask) - 1; i++)
            {
                netAddr[3]++;
                if (netAddr[3] > 255)
                {
                    netAddr[2]++;
                    netAddr[3] = 0;
                    if (netAddr[2] > 255)
                    {
                        netAddr[1]++;
                        netAddr[2] = 0;
                        if (netAddr[1] > 255)
                        {
                            netAddr[0]++;
                            netAddr[1] = 0;
                            if (netAddr[0] > 255) throw new Exception("Address overflow");
                        }
                    }
                }
                string[] arr = new string[4];
                arr[0] = netAddr[0].ToString();
                arr[1] = netAddr[1].ToString();
                arr[2] = netAddr[2].ToString();
                arr[3] = netAddr[3].ToString();
                addresses.Add(IPAddress.Parse(String.Join(".", netAddr)));
            }

            //Console.WriteLine(netAddr[0] + "." + netAddr[1] + "." + netAddr[2] + "." + netAddr[3]);

            return addresses.ToArray();
        }

        private static void ThreadSendToAll(object data)
        {
            if (data is Data)
            {
                Data d = (Data)data;
                try
                {
                    SendDirectMessage(d.message, d.ip, d.port);
                }
                catch (Exception) { }
            }
        }

        private class Data
        {
            public string message;
            public IPAddress ip;
            public int port;

            public Data(string message, IPAddress ip, int port)
            {
                this.message = message;
                this.ip = ip;
                this.port = port;
            }
        }

        public static void SendToAllHostsInNetwork(string message, string netIp, int port)
        {
            Console.WriteLine("Sending data to all host in the network");
            List<Thread> threads = new List<Thread>();
            foreach (var ip in GetAddressesInNetwork(netIp))
            {
                //SendDirectMessage(message, ip, port);
                Thread thread = new Thread(Connection.ThreadSendToAll);
                thread.Start(new Data(message, ip, port));
                threads.Add(thread);
            }

            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();
            }
        }

        public static void SendDirectMessage(string message, string ip, int port)
        {
            TcpClient client = new TcpClient(ip, port);

            int byteCount = Encoding.ASCII.GetByteCount(message);
            byte[] sendData = new byte[byteCount];

            sendData = Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client.GetStream();
            stream.Write(sendData, 0, sendData.Length);
            stream.Close();
            client.Close();
        }

        public static void SendDirectMessage(string message, IPAddress ip, int port)
        {
            TcpClient client = new TcpClient(ip.ToString(), port);
            client.ReceiveTimeout = 100;
            client.SendTimeout = 100;

            int byteCount = Encoding.ASCII.GetByteCount(message);
            byte[] sendData = new byte[byteCount];

            sendData = Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client.GetStream();
            stream.Write(sendData, 0, sendData.Length);
            stream.Close();
            client.Close();

            Console.WriteLine("Sent to " + ip.ToString());
        }
    }
}
