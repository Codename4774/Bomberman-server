using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Bomberman_client.GameClasses;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Bomberman_client;

namespace Bomberman_server
{
    public class ServerCore
    {
        public readonly IPHostEntry ipHost;
        public readonly IPAddress ipAdress;
        public readonly int portControl;
        public readonly int portData;
        
        public readonly int maxLengthQueue;
        private int bufferSize;
        public readonly IPEndPoint ipEndPointControl;
        public readonly IPEndPoint ipEndPointData;
        public readonly Socket socketListener;
        public readonly UdpClient socketSender;
        public readonly List<Socket> socketsList;
        public GameCoreServer gameCoreServer;
        public BinaryFormatter serializer;
        public MessageAnalyzer messageAnalyzer;
        private int idCounter;
        

        public ServerCore(int portControl, int portData, int maxLengthQueue, int sendFrequency)
        {
            this.ipHost = Dns.GetHostEntry("localhost");

            this.portControl = portControl;
            this.maxLengthQueue = maxLengthQueue;
            this.ipAdress = ipHost.AddressList[0];
            this.bufferSize = 2048;
            this.idCounter = 0;


            this.ipEndPointControl = new IPEndPoint(ipAdress, portControl);
            this.ipEndPointData = new IPEndPoint(IPAddress.Any, portData);

            this.socketListener = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.socketSender = new UdpClient(portData);

            this.socketsList = new List<Socket>();
            this.serializer = new BinaryFormatter();
            try
            {
                socketListener.Bind(ipEndPointControl);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void SendData()
        {
            lock (gameCoreServer.buffer)
            {

                MemoryStream data = new MemoryStream();
                serializer.Serialize(data, gameCoreServer.buffer);

                foreach (Socket client in socketsList)
                {
                    SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
                    sendArgs.SetBuffer(data.ToArray(), 0, data.ToArray().Length);
                    client.SendAsync(sendArgs);
                }
            }
        }

        private void AcceptCallback(object sender, SocketAsyncEventArgs e)
        {

        }
        private void ReceiveCallback(object sender, SocketAsyncEventArgs e)
        {
            messageAnalyzer.AnalyzeMessage(e.Buffer);
            byte[] buffer = new byte[bufferSize];
            SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += ReceiveCallback;

            eventArgs.SetBuffer(buffer, 0, bufferSize);
            (sender as Socket).ReceiveAsync(eventArgs);

        }

        private void SendId(Socket client)
        {
            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.SetBuffer(BitConverter.GetBytes(idCounter), 0, sizeof(int));
            idCounter++;
            client.SendAsync(sendArgs);
        }
        private void AddPlayerToList()
        {
            gameCoreServer.players.Add(new Player(new Point(0, 0), gameCoreServer.playerTexture, gameCoreServer.playerSize, "", gameCoreServer.DeletePlayerFromField, gameCoreServer.bombTexture, gameCoreServer.bombSize, gameCoreServer.DeleteBombFromField, idCounter - 1));
        }
        public void StartListen(object state)
        {
            socketListener.Listen(maxLengthQueue);

            while (true)
            {
                Socket newClient = socketListener.Accept();
                Console.WriteLine("New user connected to the server");

                socketsList.Add(newClient);
                SendId(newClient);
                AddPlayerToList();
                IPEndPoint tempEndPoint = new IPEndPoint(IPAddress.Any, 0);
                EndPoint temp = (EndPoint)tempEndPoint;
                byte[] buffer = new byte[bufferSize];
                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                eventArgs.Completed += ReceiveCallback;

                eventArgs.SetBuffer(buffer, 0, bufferSize);
                newClient.ReceiveAsync(eventArgs);
            }
        }
    }
}
