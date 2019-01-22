//using Newtonsoft.Json;
using ScatterSharp.Core.Api;
using ScatterSharp.UnitTests.Core;
using System;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests
{
    //public class ScatterEosUnitTests
    //{
    //    //mainnet
    //    public static readonly Network network = new Network()
    //    {
    //        blockchain = Scatter.Blockchains.EOSIO,
    //        host = "api.eossweden.se",
    //        port = 443,
    //        chainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
    //    };

    //    //btuga testnet
    //    //public static readonly Api.Network network = new Api.Network()
    //    //{
    //    //    Blockchain = Scatter.Blockchains.EOSIO,
    //    //    Host = "nodeos01.btuga.io",
    //    //    Port = 443,
    //    //    ChainId = "cf057bbfb72640471fd910bcb67639c22df9f92470936cddc1ade0e2f2e7dc4f"
    //    //};

    //    public ScatterEosUnitTestCases ScatterEosUnitTestCases { get; set; }

    //    public ScatterEosUnitTests()
    //    {
    //        var scatter = new Scatter("SCATTER-SHARP", network);
    //        ScatterEosUnitTestCases = new ScatterEosUnitTestCases(scatter, network);
    //    }

    //    public async Task Connect()
    //    {
    //        bool success = false;

    //        try
    //        {
    //            await ScatterEosUnitTestCases.Connect();
    //            success = true;
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(JsonConvert.SerializeObject(ex));
    //        }

    //        if (success)
    //            Console.WriteLine("Test Connect run successfuly.");
    //        else
    //            Console.WriteLine("Test Connect run failed.");
    //    }

    //    public async Task PushTransaction()
    //    {
    //        bool success = false;

    //        try
    //        {
    //            success = await ScatterEosUnitTestCases.PushTransaction();
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(JsonConvert.SerializeObject(ex));
    //        }

    //        if (success)
    //            Console.WriteLine("Test PushTransaction run successfuly.");
    //        else
    //            Console.WriteLine("Test PushTransaction run failed.");
    //    }
    //}
}
