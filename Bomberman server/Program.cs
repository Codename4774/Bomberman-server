using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Bomberman_client.GameClasses;
using System.Drawing;

namespace Bomberman_server
{
    class Program
    {
        static void Main(string[] args)
        {
            int width = 432;
            int height = 384;
            try
            {
                Console.WriteLine("IP:");
                string IPAddress = Console.ReadLine();
                ServerCore serverCore = new ServerCore(IPAddress, 11000, 11001, 5, 40);
                serverCore.gameCoreServer = new GameCoreServer(width, height, new Size(24, 32), new Size(24, 24), new Size(24, 24), new Size(24, 24), new Size(24, 24), (Environment.CurrentDirectory + "\\Resources\\"), serverCore.SendData);
                serverCore.messageAnalyzer = new Bomberman_client.MessageAnalyzer(serverCore.gameCoreServer);
                Console.WriteLine("Server has been started. IP - {0}, port - {1}. To end from the program press enter", serverCore.ipAdress.ToString(), serverCore.portControl);
                serverCore.gameCoreServer.startCore();
                ThreadPool.QueueUserWorkItem(serverCore.StartListen);
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                { 
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                }
            }

        }
    }
}
