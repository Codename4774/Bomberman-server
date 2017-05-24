using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

namespace Bomberman_server
{
    public class ServerCore
    {
        public readonly IPHostEntry ipHost;
        public readonly IPAddress ipAdress;
        public readonly int port;
        public readonly int maxLengthQueue;
        private int bufferSize;
        public readonly IPEndPoint ipEndPoint;
        public readonly Socket socketListener;
        public readonly List<Socket> socketsList;
        public List<SendPacketsElement> messagesList;
        private System.Timers.Timer timer;

        public ServerCore(int port, int maxLengthQueue, int sendFrequency)
        {
            this.ipHost = Dns.GetHostEntry("localhost");

            this.port = port;
            this.maxLengthQueue = maxLengthQueue;
            this.ipAdress = ipHost.AddressList[0];
            this.bufferSize = 2048;
            this.timer = new System.Timers.Timer();
            this.timer.Interval = sendFrequency;
            this.timer.Elapsed += SendOnTimer;


            this.ipEndPoint = new IPEndPoint(ipAdress, port);
            this.socketListener = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            this.socketsList = new List<Socket>();
            this.messagesList = new List<SendPacketsElement>();

            try
            {
                socketListener.Bind(ipEndPoint);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private void SendOnTimer(object sender, EventArgs e)
        {
            lock (messagesList)
            {
                SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
                sendArgs.SendPacketsElements = messagesList.ToArray();
                foreach (Socket client in socketsList)
                {
                    client.SendAsync(sendArgs);
                }
                messagesList.Clear();
            }
        }

        private void AcceptCallback(object sender, SocketAsyncEventArgs e)
        {

        }

        private void ReceiveCallback(object sender, SocketAsyncEventArgs e)
        {
            messagesList.Add(new SendPacketsElement(e.Buffer));
        }

        public void StartListen(object state)
        {
            socketListener.Listen(maxLengthQueue);
            this.timer.Enabled = true;

            while (true)
            {
                Socket newClient = socketListener.Accept();
                socketsList.Add(newClient);

                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                eventArgs.Completed += ReceiveCallback;
                eventArgs.SetBuffer(0, bufferSize);
                newClient.ReceiveAsync(eventArgs);
            }
        }
    }
}
