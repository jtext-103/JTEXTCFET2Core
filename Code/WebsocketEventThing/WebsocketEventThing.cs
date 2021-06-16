using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Log;
using System;

namespace Jtext103.CFET2.WebsocketEvent
{
    public class WebsocketEventThing : Thing, IRemoteEventHub
    {


        public WebsocketEventConfig Config { get; set; }

        /// <summary>
        /// the uri shceme
        /// </summary>
        public string Protocol => "ws";

        private ICfet2Logger logger;
        private WebsocketEventServer wsServer;
        private WebsocketEventClient wsClient;

        public WebsocketEventThing()
        {
            
        }

        public override void TryInit(object initObj)
        {
            Config = new WebsocketEventConfig(ChangetoWS(initObj.ToString()));
            logger = Cfet2LogManager.GetLogger("WsEvent@" + Path);
            WebsocketEventHandler.ParentThing = this;
        }

        public string ChangetoWS(string resource)
        {
            string host;
            if (resource.ToLower().StartsWith("http:"))
            {
                int index = resource.IndexOf('/', 7);
                if (index == -1)
                {
                    host = resource;
                }
                else
                {
                    host = resource.Substring(0, index);
                }
                //change http uri to ws uri  
                //replace http://...:port with ws://...:port+1
                //eg: http://127.0.0.1:8001 to ws://127.0.0.1:8002
                host = host.ToLower().Replace("http", "ws");
                string lastchar = (int.Parse(host.Substring(host.Length - 1)) + 1).ToString();
                host = host.Substring(0, (host.Length - 1)) + lastchar;
                return host;
            }
            return resource;
        }

        public override void Start()
        {
            wsServer = new WebsocketEventServer(Config.Host);
            wsClient = new WebsocketEventClient();
            //todo: loop thrpugh a;; resource and create end points
            var resources = MyHub.GetAllLocalResources();
            wsServer.AddEndPoint("/");
            //foreach (var resource in resources)
            //{
            //    wsServer.AddEndPoint(resource.Key);
            //}
            wsServer.StartServer();
        }

        public void Subscribe(Token token, EventFilter filter, Action<EventArg> handler)
        {
            wsClient.SubscribeAync(filter, token, handler);
        }

        public void Unsbscribe(Token token)
        {
            wsClient.Unsubscribe(token);
        }

        public void Dispose()
        {
            wsClient.Dispose();
            wsServer.Dispose();
        }
    }
}
