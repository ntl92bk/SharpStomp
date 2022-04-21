using System;
namespace SharpStomp.Core
{
    public interface ISocket
    {
        void Open();
        void Send(string data);
    }
}
