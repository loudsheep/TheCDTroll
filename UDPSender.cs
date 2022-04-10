using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TheCDTrollGUI
{
    class UDPSender : UdpClient
    {
        public UDPSender() : base()
        {
            //Calls the protected Client property belonging to the UdpClient base class.
            Socket s = this.Client;
            //Uses the Socket returned by Client to set an option that is not available using UdpClient.
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }

        public UDPSender(IPEndPoint ipLocalEndPoint) : base(ipLocalEndPoint)
        {
            //Calls the protected Client property belonging to the UdpClient base class.
            Socket s = this.Client;
            //Uses the Socket returned by Client to set an option that is not available using UdpClient.
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }

        public static void SendBroadcastMessage(IPAddress sourceIPAddress, int port, string message)
        {
            IPEndPoint localEndPoint = new IPEndPoint(sourceIPAddress, 0);
            IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
            UDPSender sendUdpClient = new UDPSender(localEndPoint);

            int byteCount = Encoding.ASCII.GetByteCount(message);
            byte[] dataToSend = Encoding.ASCII.GetBytes(message);

            sendUdpClient.Send(dataToSend, byteCount, targetEndPoint);
        }

        public static void SendMessageUDP(IPAddress dst, int port, string message)
        {
            UDPSender sendUdpClient = new UDPSender();
            Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            IPEndPoint dstAddr = new IPEndPoint(dst, port);

            sendUdpClient.Send(sendBytes, sendBytes.Length, dstAddr);
        }
    }
}
