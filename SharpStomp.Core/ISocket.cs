using System;
namespace SharpStomp.Core
{
    public delegate void OnSocketOpenSuccess();
    public delegate void OnSocketOpenError(string error);
    public delegate void OnMessage(string message);

    public interface ISocket
    {
        event OnMessage onMessage;
        void Open(OnSocketOpenSuccess onSuccess, OnSocketOpenError onError);
        void Send(string data);
        void Close();
    }
}
