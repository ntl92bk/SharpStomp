using System;
using System.Collections.Generic;

namespace SharpStomp.Core
{
    public class StompMessage
    {
        public string Command { get; private set; }
        public readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        public string Body { get; private set; }

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        public StompMessage(string command) : this(command, new Dictionary<string, string>(), string.Empty)
        {
            
        }

        public StompMessage(string command,string body) : this(command,new Dictionary<string,string>(),body)
        {

        }

        public StompMessage(string command,Dictionary<string,string> headers) : this(command,headers,string.Empty)
        {

        }

        public StompMessage(string command,Dictionary<string,string> headers,string body)
        {
            Command = command;
            _headers = headers ?? new Dictionary<string, string>();
            Body = body;
        }

        public string this[string key]
        {
            get
            {
                return _headers.ContainsKey(key) ? _headers[key] : string.Empty;
            }
            set
            {
                _headers[key] = value;
            }
        }
        
    }
}
