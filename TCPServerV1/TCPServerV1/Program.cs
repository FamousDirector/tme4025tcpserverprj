using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static TCPServerV1.TCPServer;

namespace TCPServerV1
{
    public class EntryPoint
    {
        public static int Main(string[] args)
        {
            AsynchronousSocketListener.StartListening();
            return 0;
        }
    }
}

