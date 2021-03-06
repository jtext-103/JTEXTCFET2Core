using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtext103.CFET2.Core.Event;
using FluentAssertions;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.Event
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class EventUnSubscribeTest
    {
        EventHub hub;
        int counter = 0;
        object lockObj = new object();
        object lockObj2 = new object();
        object lockObj3 = new object();
        private int counter2;
        private int counter3;

        [TestInitialize]
        public void init()
        {
            hub = new EventHub();
            counter = 0;
            lockObj = new object();
            lockObj2 = new object();
            lockObj3 = new object();
            counter2 = 0;
            counter3 = 0;
            hub.Init();
        }

        [TestCleanup]
        public void clean()
        {
            hub.Dispose();
            counter = 0;
        }

        [TestMethod]
        public void ShouldStopAfterUnsubscribe()
        {
            //arrange
            var token=hub.Subscribe(new EventFilter(@"/t/s1", "changed"), eventHandle);
            var token2=hub.Subscribe(new EventFilter(@"/t/s2", "changed"), eventHandle2);
            hub.Subscribe(new EventFilter(@"/t/s(\w)*", "changed"), eventHandle3);

            //act
            hub.Publish("/t/s1", "changed", 1);
            hub.Publish("/t/s1", "changed", 1);
            hub.Publish("/t/s2", "changed", 1);
            hub.Publish("/t/s2", "changed", 1);
            Thread.Sleep(100);
            token.Dispose();
            token2.Dispose();
            hub.Publish("/t/s1", "changed", 1);
            hub.Publish("/t/s1", "changed", 1);
            hub.Publish("/t/s2", "changed", 1);
            Thread.Sleep(2000);
            //assert
            counter.Should().Be(2);
            counter2.Should().Be(2);
            counter3.Should().Be(7);


        }

       
        public void eventHandle(EventArg e)
        {

            lock (lockObj)
            { Thread.Sleep(100); counter += (int)e.Sample.ObjectVal; }
        }

        public void eventHandle2(EventArg e)
        {

            lock (lockObj2)
            { Thread.Sleep(50); counter2 += (int)e.Sample.ObjectVal; }
        }

        public void eventHandle3(EventArg e)
        {

            lock (lockObj3)
            { counter3 += (int)e.Sample.ObjectVal; Thread.Sleep(200); }
        }

    }
}
