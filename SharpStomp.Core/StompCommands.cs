using System;
namespace SharpStomp.Core
{
    public class StompCommands
    {
        public const string CONNECT = "CONNECT";
        public const string DISCONNECT = "DISCONNECT";        
        public const string SUBSCRIBE = "SUBSCRIBE";
        public const string UNSUBSCRIBE = "UNSUBSCRIBE";
        public const string SEND = "SEND";
        public const string CONNECTED = "CONNECTED";
        public const string MESSAGE = "MESSAGE";
        public const string ERROR = "ERROR";
    }
}
