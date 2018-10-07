# scatter-sharp
Scatter C# library to interact with ScatterDesktop / ScatterMobile

### Prerequisite to build

Visual Studio 2017 

### Instalation


### Usage

#### Configuration

In order to use scatter you need to create a instance with **AppName** and a **Network** configuration, connect to scatter application, request a **Identity** and then fetch a new **Eos** instance.

Example:

```csharp
var network = new Api.Network()
{
    Blockchain = Scatter.Blockchains.EOSIO,
    Host = "api.eossweden.se",
    Port = 443,
    ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
};

var scatter = new Scatter("MY-APP-NAME", network);

await scatter.Connect();

var identity = await Scatter.GetIdentity(new Api.IdentityRequiredFields()
{
    Accounts = new List<Api.Network>()
    {
        network
    },
    Location = new List<Api.LocationFields>(),
    Personal = new List<Api.PersonalFields>()
});

var eos = Scatter.Eos();

... **Use all eos api methods as usual from EosSharp** ...
```

#### Scatter Api methods

**TODO**
