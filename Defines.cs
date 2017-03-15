using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.TcpMonitor
{
    public static class Defines
    {
        public const int TCP_PORT = 55555;
        public const int HANDSHAKE_TIMEOUT = 5000; //(milliseconds)
        public const int READ_BUFFER_SIZE = 1024;
        public const string ACTIVE_CONNECTIONS_MSG = "the maximum number of active connection has reached";
        public const string CLIENT_MSG_HANDSHAKE_COMMAND = "HELO";
        public const string CLIENT_MSG_HANDSHAKE_COUNTS = "COUNT";
        public const string CLIENT_MSG_ACTIVE_CONNECTIONS = "CONNECTIONS";
        public const string CLIENT_MSG_PRIME = "PRIME";
        public const string CLIENT_MSG_TERMINATE = "TERMINATE";
        public const string SERVER_MSG_HANDSHAKE_REPLY = "HI";
        public const string SERVER_MSG_TERMINATE_REPLY = "BYE";
        public const int PRIME_NUMBER = 11; //Note: the project requirement used the number 142339, but 142339 is not a prime number
        public const string INVALID_HANDSHAKE_COMMAND = "invalid handshake command";
    }
}
