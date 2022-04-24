using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using SharpStomp.Core;
using WebSocket4Net;

namespace SharpStomp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

            Console.WriteLine($"Socket uri: {settings.SocketUri}");

            new Thread(() =>
            {
                var socket = new WebSocket4NetAdapter(new WebSocket(settings.SocketUri));
                var client = new StompClient(socket);

                Thread.Sleep(2000);
                client.Connect(new System.Collections.Generic.Dictionary<string, string>(), msg =>
                {
                    Console.WriteLine("Connected");
                }, error =>
                {
                    Console.WriteLine($"Error:{error}");
                });

                client.onMessageReceived += msg =>
                {
                    Console.WriteLine($">Receive message : {msg.Body}");
                };

                Thread.Sleep(1000);
                var subId = client.Subcribe("/sub/chat/room/9", null);

                Thread.Sleep(1000);
                client.Send("/pub/chat/message", "{\"chatRoomId\":9,\"sender\":\"Hiha 1\",\"message\":\"hello\",\"messageType\":\"TALK\"}");
                Thread.Sleep(1000);
                client.Send("/pub/chat/message", "{\"chatRoomId\":9,\"sender\":\"Hiha 2\",\"message\":\"hello\",\"messageType\":\"TALK\"}");
                Thread.Sleep(1000);
                client.UnSubcribe(subId);
                Thread.Sleep(1000);
                client.Send("/pub/chat/message", "{\"chatRoomId\":9,\"sender\":\"Hiha 3\",\"message\":\"hello\",\"messageType\":\"TALK\"}");

                client.Dispose();
            }).Start();

            Console.ReadKey();
        }

        public class WebSocket4NetAdapter : ISocket
        {
            readonly WebSocket _socket;
            OnSocketOpenSuccess _onSocketOpenSuccess;
            OnSocketOpenError _onSocketOpenError;

            public event OnMessage onMessage;

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
                _onSocketOpenSuccess?.Invoke();
            }

            private void OnSocketError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
            {
                Console.WriteLine($"Socket Error :{e.Exception.Message}");
                _onSocketOpenError?.Invoke(e.Exception.Message);
            }

            void OnSocketMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
            {
                Console.WriteLine($"RECEIVE RAW : {e.Message}");
                onMessage?.Invoke(e.Message);
            }

            public void Send(string data)
            {
                Console.WriteLine($"SEND RAW : {data}");
                _socket.Send(data);
            }

            public void Open(OnSocketOpenSuccess onSuccess, OnSocketOpenError onError)
            {
                _onSocketOpenSuccess = onSuccess;
                _onSocketOpenError = onError;
                Console.WriteLine($"Open");
                _socket.Open();
            }

            public void Close()
            {
                Console.WriteLine($"Close");
                _socket.Close();
            }
        }
    }
}
