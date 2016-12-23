using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPServerV1
{
    static class DataClasses
    {
        public class ControllerData
        {
            public string UID { get; set; }
            public int RelayState { get; set; }
            public int Temperature { get; set; }
            public int Power { get; set; }
        }
    }
}
