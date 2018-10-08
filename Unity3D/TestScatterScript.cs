using EosSharp.Api.v1;
using ScatterSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TestScatterScript : MonoBehaviour
{
    public async void OnMouseDown()
    {
        await PushTransaction();
    }

    private async Task PushTransaction()
    {
        var network = new ScatterSharp.Api.Network()
        {
            Blockchain = Scatter.Blockchains.EOSIO,
            Host = "api.eossweden.se",
            Port = 443,
            ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
        };

        var scatter = new Scatter("UNITY-SCATTER", network);

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

        var result = await eos.CreateTransaction(new Transaction()
        {
            Actions = new List<EosSharp.Api.v1.Action>()
            {
                new EosSharp.Api.v1.Action()
                {
                    Account = "eosio.token",
                    Authorization = new List<PermissionLevel>()
                    {
                        new PermissionLevel() {Actor = "tester112345", Permission = "active" }
                    },
                    Name = "transfer",
                    Data = new { from = "tester112345", to = "tester212345", quantity = "0.0001 EOS", memo = "Unity 3D hello crypto world!" }
                }
            }
        });

        print(result);
    }
}
