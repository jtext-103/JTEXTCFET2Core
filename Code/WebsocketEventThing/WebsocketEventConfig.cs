using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jtext103.CFET2.WebsocketEvent
{
    public class WebsocketEventConfig
    {
        public string Host { get; set; }

        public WebsocketEventConfig(string host)
        {
            Host = host;
        }
    }
}
