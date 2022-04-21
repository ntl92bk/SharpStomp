using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpStomp.Core
{
    public static class StompMessageSerializer
    {
        public static char LF = Convert.ToChar(10);
        public static char CR = Convert.ToChar(13);
        public static char NULL = Convert.ToChar(0);

        public static string Serialize(StompMessage message)
        {
            var buffer = new StringBuilder();

            buffer.Append($"{message.Command}{LF}");

            if (message.Headers != null)
            {
                foreach (var header in message.Headers)
                {
                    buffer.Append($"{header.Key}:{header.Value}{LF}");
                }
            }

            buffer.Append(LF);
            buffer.Append(message.Body);
            buffer.Append(NULL);

            return buffer.ToString();
        }

        public static StompMessage Deserialize(string message)
        {
            var reader = new StringReader(message);

            var command = reader.ReadLine();

            var headers = new Dictionary<string, string>();

            var header = reader.ReadLine();
            while (!string.IsNullOrEmpty(header))
            {
                var split = header.Split(':');
                if (split.Length == 2) headers[split[0].Trim()] = split[1].Trim();
                header = reader.ReadLine() ?? string.Empty;
            }

            var body = reader.ReadToEnd() ?? string.Empty;
            body = body.TrimEnd(NULL, CR, LF);

            return new StompMessage(command, headers, body);
        }


    }
}
