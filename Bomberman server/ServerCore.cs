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
        public MessageAnalyzerServer messageAnalyzer;
        private int idCounter;
        

        public ServerCore(string host, int portControl, int portData, int maxLengthQueue, int sendFrequency)
        {
            this.portControl = portControl;
            this.maxLengthQueue = maxLengthQueue;
            this.ipAdress = IPAddress.Parse(host);
            this.bufferSize = 1024 * 1024;
            this.idCounter = 0;


            this.ipEndPointControl = new IPEndPoint(ipAdress, portControl);

            this.socketListener = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
            lock (gameCoreServer.objectsList)
            {

                MemoryStream data = new MemoryStream();
                ObjectsLists temp = gameCoreServer.objectsList;
                serializer.Serialize(data, temp);
                data.Flush();
                foreach (Socket client in socketsList)
                {
                    SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
                    sendArgs.UserToken = temp;
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
        private void AddPlayerToList(string playerName)
        {
            Point newLocation = gameCoreServer.spawnPoints[gameCoreServer.randomGen.Next(gameCoreServer.spawnPoints.Count - 1)];

            gameCoreServer.objectsList.players.Add(new Player(newLocation, gameCoreServer.playerSize, playerName, gameCoreServer.DeletePlayerFromField, gameCoreServer.bombSize, gameCoreServer.DeleteBombFromField, idCounter - 1));
        }
        public void StartListen(object state)
        {
            socketListener.Listen(maxLengthQueue);

            while (true)
            {
                Socket newClient = socketListener.Accept();
                Console.WriteLine("New user connected to the server");

                socketsList.Add(newClient);

                byte[] buffer = new byte[bufferSize];
                newClient.Receive(buffer);
                MemoryStream playerNameStream = new MemoryStream(buffer);
                string playerName = (string)serializer.Deserialize(playerNameStream);
                SendId(newClient);
                AddPlayerToList(playerName);
                //IPEndPoint tempEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //EndPoint temp = (EndPoint)tempEndPoint;

                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                eventArgs.Completed += ReceiveCallback;

                eventArgs.SetBuffer(buffer, 0, bufferSize);
                newClient.ReceiveAsync(eventArgs);
            }
        }
    }
}
