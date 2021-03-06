using Jtext103.CFET2.CFET2App.ExampleThings;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.NancyHttpCommunicationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.BasicThings;
using Nancy.Conventions;
using Nancy;
using Jtext103.CFET2.Core.Middleware.Basic;
using Jtext103.CFET2.CFET2App.DynamicLoad;
using Jtext103.CFET2.Core.Config;
using Jtext103.CFET2.WebsocketEvent;

namespace Jtext103.CFET2.CFET2App
{
    public partial class Cfet2Program : CFET2Host
    {
        private void AddThings()
        {


            //------------------------------Pipeline------------------------------//
            MyHub.Pipeline.AddMiddleware(new ResourceInfoMidware());
            MyHub.Pipeline.AddMiddleware(new NavigationMidware());

            GlobalConfig.Populate("./GlobalConfig.json");

            //------------------------------Nancy HTTP通信模块------------------------------//
            var nancyCM = new NancyCommunicationModule(GlobalConfig.HostUri, GlobalConfig.Accept);
            MyHub.TryAddCommunicationModule(nancyCM);

            //you can add Thing by coding here

            //------------------------------Custom View------------------------------//
            var customView = new CustomViewThing();
            MyHub.TryAddThing(customView, "/", "customView", "./CustomView");

            var remoteHub = new WebsocketEventThing();
            MyHub.TryAddThing(remoteHub, @"/", "WsEvent", GlobalConfig.HostUri);
            MyHub.EventHub.RemoteEventHubs.Add(remoteHub);

            //If you don't want dynamic load things, please comment out the line below
            var loader = new DynamicThingsLoader(this);

            //var fakeai = new FakeAIThing();
            //MyHub.TryAddThing(fakeai, "/", "fakeai", 16);

        }
    }
}
