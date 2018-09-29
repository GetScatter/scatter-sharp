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
            Scatter = new Scatter("TESTAPP");
        }

        [TestMethod]
        public async Task Connect()
        {
            await Scatter.Connect(Host);
        }

        [TestMethod]
        public async Task GetVersion()
        {
            await Scatter.Connect(Host);
            Console.WriteLine(await Scatter.GetVersion());
        }

        [TestMethod]
        public async Task GetIdentity()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task GetIdentityFromPermissions()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task ForgetIdentity()
        {
            await Scatter.Connect(Host);
            Console.WriteLine(await Scatter.ForgetIdentity());
        }

        [TestMethod]
        public async Task Authenticate()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task GetArbitrarySignature()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task GetPublicKey()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task LinkAccount()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task HasAccountFor()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task SuggestNetwork()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task RequestTransfer()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task RequestSignature()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task CreateTransaction()
        {
            await Scatter.Connect(Host);
            throw new NotImplementedException();
        }
    }
}
