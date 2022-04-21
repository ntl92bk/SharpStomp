using System;
using System.Threading;
using SharpStomp.Core;
using WebSocket4Net;

namespace SharpStorm.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            new Thread(() =>
            {
                var uri = @"";
                var socket = new WebSocket4NetAdapter(new WebSocket(uri));
                var client = new StompClient(socket);

                Thread.Sleep(2000);
                client.Connect(new System.Collections.Generic.Dictionary<string, string>(), null);
            }).Start();

            Console.ReadKey();
        }

        public class WebSocket4NetAdapter : ISocket
        {
            readonly WebSocket _socket;

            public WebSocket4NetAdapter(WebSocket socket)
            {
                _socket = socket;
                _socket.Opened += OnSocketOpened;
                _socket.Error += OnSocketError;
                _socket.MessageReceived += OnSocketMessageReceived;
            }

            private void OnSocketOpened(object sender, EventArgs e)
            {
                Console.WriteLine("Opened");
            }

            private void OnSocketError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
            {
                Console.WriteLine($"Socket Error :{e.Exception.Message}");
            }

            void OnSocketMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
            {
                Console.WriteLine(e.Message);
            }

            public void Open()
            {
                Console.WriteLine($"Open");
                _socket.Open();
            }

            public void Send(string data)
            {
                Console.WriteLine($"Send:{data}");
                _socket.Send(data);
            }
        }
    }
}
