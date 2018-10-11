# scatter-sharp
Scatter C# library to interact with ScatterDesktop / ScatterMobile

### Prerequisite to build

Visual Studio 2017 

### Instalation
scatter-sharp is now available throught nuget https://www.nuget.org/packages/scatter-sharp
```
Install-Package scatter-sharp
```

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
    Protocol = "https",
    ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
};

var scatter = new Scatter("MY-APP-NAME", network);

await scatter.Connect();

var identity = await scatter.GetIdentity(new Api.IdentityRequiredFields()
{
    Accounts = new List<Api.Network>()
    {
        network
    },
    Location = new List<Api.LocationFields>(),
    Personal = new List<Api.PersonalFields>()
});

var eos = scatter.Eos();

... **Use all eos api methods as usual from eos-sharp** ...

scatter.Dispose();
```

#### App Storage Provider

Scatter uses a appKey and Nonce to help pair your application with the users permissions. By default this information is stored on memory but you may want to save on disk to be reused later.

Create a new Scatter instance and configure a FileStorageProvider with the target filePath.
```csharp
var fileStorage = new FileStorageProvider(filePath);

using (var scatter = new Scatter("UNITY-SCATTER-JUNGLE", network, fileStorage))
{
    await scatter.Connect();
    ....    
}
```

#### Scatter Api methods

- **GetVersion**
Gets the Scatter version
```csharp
string version = await scatter.GetVersion();
```

- **GetIdentity**
Prompts the users for an Identity if there is no permission, otherwise returns the permission without a prompt based on origin.
```csharp
Identity identity = await scatter.GetIdentity(new Api.IdentityRequiredFields() {
    Accounts = new List<Api.Network>()
    {
        network
    },
    Location = new List<Api.LocationFields>(),
    Personal = new List<Api.PersonalFields>()
});
```
Returns:
```csharp
public class Identity
{
    public string Hash { get; set; }
    public string PublicKey { get; set; }
    public string Name { get; set; }
    public bool Kyc { get; set; }
    public List<IdentityAccount> Accounts { get; set; }
}
```

- **GetIdentityFromPermissions**
Checks if an Identity has permissions and return the identity based on origin.
```csharp
Identity identity = await scatter.GetIdentityFromPermissions();
```
Returns:
```csharp
public class Identity
{
    public string Hash { get; set; }
    public string PublicKey { get; set; }
    public string Name { get; set; }
    public bool Kyc { get; set; }
    public List<IdentityAccount> Accounts { get; set; }
}
```

- **ForgetIdentity**
Removes the identity permission for an origin from the user's Scatter, effectively logging them out.
```csharp
bool result = await scatter.ForgetIdentity();
```

- **Authenticate**
Signs the origin with the Identity's private key.
```csharp
string signature = await scatter.Authenticate();
```

- **GetArbitrarySignature**
Requests an arbitrary signature of data.
```csharp
string signature = await scatter.GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false);
```

- **GetPublicKey**
Allows apps to request that the user provide a user-selected Public Key to the app. ( ONBOARDING HELPER )
```csharp
string pubKey = await scatter.GetPublicKey(string blockchain);
```

- **LinkAccount**
Allows the app to suggest that the user link new accounts on top of public keys ( ONBOARDING HELPER )
```csharp
bool result = await scatter.LinkAccount(string publicKey);
```

- **HasAccountFor**
Allows dapps to see if a user has an account for a specific blockchain. DOES NOT PROMPT and does not return an actual account, just a boolean.
```csharp
bool result = await scatter.HasAccountFor();
```

- **SuggestNetwork**
Prompts the user to add a new network to their Scatter.
```csharp
bool result = await scatter.SuggestNetwork();
```






