using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpStomp.Core
{
    public class StompClient
    {
        readonly ISocket _socket;

        public StompClient(ISocket socket)
        {
            _socket = socket;
            _socket.Open();
        }

        public delegate void OnCommandResult(string error, StompMessage message);

        public void Connect(Dictionary<string, string> headers, OnCommandResult onCommandResult)
        {
            var msg = new StompMessage(StompCommands.CONNECT, headers);
            _socket.Send(StompMessageSerializer.Serialize(msg));
        }
        public void Disconnect()
        {

        }

        public void Send(string path, string content)
        {

        }
        public void Send(string path, Dictionary<string, string> headers, string content)
        {

        }

        public void Subcribe(string path, OnCommandResult onCommandResult)
        {

        }
        public void Subcribe(string path, Dictionary<string, string> headers, OnCommandResult onCommandResult)
        {

        }

        public void UnSubcribe(string path)
        {

        }
    }
}