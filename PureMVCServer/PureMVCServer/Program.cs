using PureNet.Network;
using System;

namespace PureMVCServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AsncTcpServer server = new AsncTcpServer();
            server.Start("127.0.0.1", 25565);

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "quit")
                    return;
            }
        }
    }
}
