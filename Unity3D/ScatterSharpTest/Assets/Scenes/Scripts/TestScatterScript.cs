using EosSharp.Api.v1;
using Newtonsoft.Json;
using ScatterSharp;
using ScatterSharp.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestScatterScript : MonoBehaviour
{
    public async void PushTransaction()
    {
        try
        {
            var network = new ScatterSharp.Api.Network()
            {
                Blockchain = Scatter.Blockchains.EOSIO,
                Host = "api.jungle.alohaeos.com",
                Port = 443,
                Protocol = "https",
                ChainId = "038f4b0fc8ff18a4f0842a8f0564611f6e96e8535901dd45e43ac8691a1c4dca"
            };

            var fileStorage = new FileStorageProvider(Application.persistentDataPath + "/scatterapp.dat");

            using (var scatter = new Scatter("UNITY-SCATTER-JUNGLE", network, fileStorage))
            {
                await scatter.Connect();

                await scatter.GetIdentity(new ScatterSharp.Api.IdentityRequiredFields()
                {
                    Accounts = new List<ScatterSharp.Api.Network>()
                    {
                        network
                    },
                    Location = new List<ScatterSharp.Api.LocationFields>(),
                    Personal = new List<ScatterSharp.Api.PersonalFields>()
                });

                var eos = scatter.Eos();
                
                var account = scatter.Identity.Accounts.First();

                var result = await eos.CreateTransaction(new Transaction()
                {
                    Actions = new List<EosSharp.Api.v1.Action>()
                    {
                        new EosSharp.Api.v1.Action()
                        {
                            Account = "eosio.token",
                            Authorization =  new List<PermissionLevel>()
                            {
                                new PermissionLevel() {Actor = account.Name, Permission = account.Authority }
                            },
                            Name = "transfer",
                            Data = new { from = account.Name, to = "eosio", quantity = "0.0001 EOS", memo = "Unity 3D hello crypto world!" }
                        }
                    }
                });

                print(result);
            }
        }
        catch (Exception ex)
        {
            print(JsonConvert.SerializeObject(ex));
        }
    }
}
