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

        [TestMethod]
        public async Task GetVersion()
        {
            await Scatter.Connect(Host);
            await Scatter.GetVersion();
            Thread.Sleep(20000);
        }

        [TestMethod]
        public void GetIdentity()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetIdentityFromPermissions()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ForgetIdentity()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Authenticate()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetArbitrarySignature()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetPublicKey()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void LinkAccount()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void HasAccountFor()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SuggestNetwork()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void RequestTransfer()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void RequestSignature()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateTransaction()
        {
            throw new NotImplementedException();
        }
    }
}
