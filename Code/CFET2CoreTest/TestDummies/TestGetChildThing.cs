using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;

namespace Jtext103.CFET2.Core.Test.TestDummies
{
    public class TestGetChildThing : Thing
    {
        //status
        [Cfet2Status("ram")]
        public float GetRam()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
            return ramCounter.NextValue();
        }

        [Cfet2Status("defaultStatus")]
        public string GetDefaultStatus()
        {
            return this.Path + @"/defaultStatus";
        }
    }
}
