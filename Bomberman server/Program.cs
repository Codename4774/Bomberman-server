using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Bomberman_server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServerCore serverCore = new ServerCore(11000, 5, 40);
                Console.WriteLine("Server has been started. IP - {0}, port - {1}. To end from the program press enter", serverCore.ipHost.AddressList[0], serverCore.port);

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
