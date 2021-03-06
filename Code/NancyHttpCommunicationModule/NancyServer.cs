using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Log;
using Nancy;
using Nancy.Hosting.Self;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    public class NancyServer
    {
        //用来与 CFET2 进行通信
        static public Hub TheHub;
        public Uri UriHost;
        private ICfet2Logger logger = Cfet2LogManager.GetLogger("NancyModule");

        public NancyServer(Hub hub, Uri myUriHost)
        {
            TheHub = hub;
            UriHost = myUriHost;
        }

        public void Start()
        {
            Task.Factory.StartNew(() => serverLoop(), TaskCreationOptions.LongRunning);
            //Task.Run(() => serverLoop());
            logger.Info("NancyTaskRun ");
        }

        public void serverLoop()
        {
            HostConfiguration hostConfigs = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            using (var host = new NancyHost(UriHost, new CustomBootstrapper(), hostConfigs))
            {
                host.Start();
                Console.WriteLine("Running on " + UriHost);
                logger.Info("Running on " + UriHost);
                while (true)
                {
                    Thread.Sleep(10000);
                }
            }

        }
    }
}
