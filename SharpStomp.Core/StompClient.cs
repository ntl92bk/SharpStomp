﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpStomp.Core
{
    public class StompClient : IDisposable
    {
        readonly ISocket _socket;
        HandleCommandSuccess _onConnectCommandSuccess;
        HandleCommandError _onConnectCommandError;

        public StompClient(ISocket socket)
        {
            _socket = socket;
            _socket.onMessage += OnMessage;
        }

        public delegate void HandleCommandSuccess(StompMessage message);
        public delegate void HandleCommandError(string error);
        public delegate void HandleMessage(StompMessage msg);

        public event HandleMessage onMessageReceived;

        public bool IsConnected => _connected;

        bool _connecting = false;
        bool _connected = false;

        public void Connect(Dictionary<string, string> headers, HandleCommandSuccess onCommandSuccess, HandleCommandError onCommandError)
        {
            if (_connected)
            {
                Console.WriteLine("Already connected");
                return;
            }

            if (_connecting)
            {
                Console.WriteLine("Cannot call connecting in parallel");
                return;
            }

            _onConnectCommandSuccess = onCommandSuccess;
            _onConnectCommandError = onCommandError;

            _connecting = true;
            _socket.Open(() =>
            {
                var msg = new StompMessage(StompCommands.CONNECT, headers);
                msg["accept-version"] = "1.2";
                _socket.Send(StompMessageSerializer.Serialize(msg));
            },
            err =>
            {
                _connecting = false;
                _onConnectCommandError?.Invoke(err);
            });
        }

        public void Disconnect()
        {
            _socket.Close();
            _connected = false;
        }

        public void Send(string path, string content)
        {
            Send(path, null, content);
        }

        public void Send(string path, Dictionary<string, string> headers, string content)
        {
            if (!_connected)
            {
                throw new NotConnectedException();
            }

            var msg = new StompMessage(StompCommands.SEND, headers, content);
            msg["destination"] = path;
            msg["content-type"] = "application/json";
            _socket.Send(StompMessageSerializer.Serialize(msg));
        }

        public int Subcribe(string path, HandleCommandSuccess onCommandSuccess)
        {
            return Subcribe(path, null, onCommandSuccess);
        }

        int _subId;

        public int Subcribe(string path, Dictionary<string, string> headers, HandleCommandSuccess onCommandSuccess)
        {
            if (!_connected)
            {
                throw new NotConnectedException();
            }

            var msg = new StompMessage(StompCommands.SUBSCRIBE, headers, string.Empty);
            msg["destination"] = path;
            msg["id"] = $"sub-{++_subId}";
            _socket.Send(StompMessageSerializer.Serialize(msg));
            return _subId;
        }

        public void UnSubcribe(long subId)
        {
            if (!_connected)
            {
                throw new NotConnectedException();
            }

            var msg = new StompMessage(StompCommands.UNSUBSCRIBE, string.Empty);
            msg["id"] = $"sub-{subId}";
            _socket.Send(StompMessageSerializer.Serialize(msg));
        }

        void OnMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                var stompMessage = StompMessageSerializer.Deserialize(message);

                if (stompMessage.Command == StompCommands.CONNECTED)
                {
                    _connected = true;
                    _connecting = false;

                    _onConnectCommandSuccess?.Invoke(stompMessage);
                }
                else if (stompMessage.Command == StompCommands.MESSAGE)
                {
                    onMessageReceived?.Invoke(stompMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        bool _disposed = false;
        void Dispose(bool isDispose)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (isDispose)
            {
                _socket.Close();
            }
        }

        ~StompClient()
        {
            Dispose(false);
        }
    }

    public class NotConnectedException : Exception
    {

    }
}