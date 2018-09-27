using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests
{
    [TestClass]
    public class ScatterUnitTests
    {
        public string Host = "127.0.0.1:50005";
        public Scatter Scatter { get; set; }

        public ScatterUnitTests()
        {
            Scatter = new Scatter();
        }

        [TestMethod]
        public async Task Connect()
        {
            await Scatter.Connect(Host);
            Thread.Sleep(20000);
        }
    }
}
