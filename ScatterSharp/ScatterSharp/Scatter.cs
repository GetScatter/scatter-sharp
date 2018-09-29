using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace ScatterSharp
{
    public class Scatter
    {
        private readonly string WSURI = "ws://{0}/socket.io/?EIO=3&transport=websocket";
        SocketService SocketService { get; set; }

        public Scatter()
        {
            SocketService = new SocketService();
        }

        //const throwNoAuth = () => {
        //    if (!holder.scatter.isExtension && !SocketService.isConnected())
        //        throw new Error('Connect and Authenticate first - scatter.connect( pluginName )');
        //};

        public Task Connect(string host, CancellationToken? cancellationToken = null)
        {
            return SocketService.Link(new Uri(string.Format(WSURI, host)), cancellationToken);
        }

        public async Task<string> GetVersion()
        {
            var x = await SocketService.SendApiRequest(new ScatterApiRequest()
            {
                Type = "getVersion",
                Payload = new { }
            });

            return "";
        }

        public void GetIdentity(/*requiredFields*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'getOrRequestIdentity',
            //payload:
            //    {
            //        fields: requiredFields
            //}
            //}).then(id => {
            //    if (id) this.identity = id;
            //    return id;
            //});
        }

        public void GetIdentityFromPermissions()
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'identityFromPermissions',
            //payload: { }
            //}).then(id => {
            //    if (id) this.identity = id;
            //    return id;
            //});
        }

        public void ForgetIdentity()
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'forgetIdentity',
            //payload: { }
            //}).then(res => {
            //    this.identity = null;
            //    return res;
            //});
        }

        public void Authenticate(/*nonce*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'authenticate',
            //payload: { nonce }
            //});
        }

        public void GetArbitrarySignature(/*publicKey, data, whatfor = '', isHash = false*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'requestArbitrarySignature',
            //payload:
            //    {
            //        publicKey,
            //    data,
            //    whatfor,
            //    isHash
            //}
            //});
        }

        public void GetPublicKey(/*blockchain*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'getPublicKey',
            //payload: { blockchain }
            //});
        }

        public void LinkAccount(/*publicKey, network*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'linkAccount',
            //payload: { publicKey, network }
            //});
        }

        public void HasAccountFor(/*network*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'hasAccountFor',
            //payload:
            //    {
            //        network
            //}
            //});
        }

        public void SuggestNetwork(/*network*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'requestAddNetwork',
            //payload:
            //    {
            //        network
            //}
            //});
        }

        public void RequestTransfer(/*network, to, amount, options = { }*/)
        {
            throw new NotImplementedException();
            //const payload = { network, to, amount, options };
            //return SocketService.sendApiRequest({
            //    type:'requestTransfer',
            //    payload
            //});
        }

        public void RequestSignature(/*payload*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'requestSignature',
            //        payload
            //    });
        }

        public void CreateTransaction(/*blockchain, actions, account, network*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'createTransaction',
            //        payload:
            //    {
            //        blockchain,
            //            actions,
            //            account,
            //            network
            //        }
            //});
        }
    }
}
