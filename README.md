
REMME .NET Client
==========
[![NuGet version](https://badge.fury.io/nu/REMME.Auth.Client.svg)](https://badge.fury.io/nu/REMME.Auth.Client)

**An open source .NET integration library for REMChain, simplifying the access and interaction with REMME nodes both public or permissioned.**

**NOTE**: Consider using **0.2.*** library version if you communicate with REMME node via REST API (current REMME testnet), 
If you communicate with REMME node via JSON RPC consider using **0.3.*** library version (if you build node locally from the source code)

How to use
----------
1. Install and run REMME node with required REST API methods  enabled. 
You can check out how to do that at [REMME core repo](https://github.com/Remmeauth/remme-core/). 
*Note: you can enable/disable methods by modifying **REMME_REST_API_AVAILABLE_METHODS** eviroment variable at the .env file.*

2. Install the latest version of library to your .NET project
```
PM > Install-Package REMME.Auth.Client
```

3. Run methods of **RemmeClient** class to interract with REMME node. 

Examples
------------
#### Account management
```csharp
var newRemmeAccount = new RemmeAccount();
//You can get account data using next properties
//newRemmeAccount.PublicKeyHex
//newRemmeAccount.PrivateKeyHex
//newRemmeAccount.Address
```
#### Creating client
```csharp
//Addresses of Docker container with runing REMME node
var networkConfig = new RemmeNetworkConfig
{
  NodeAddress = "192.168.99.100",
  SslMode = false,
};
var privateKeyHex ="78a8f39be4570ba8dbb9b87e6918a4c2559bc4e8f3206a0a755c6f2b659a7850";

var client = new RemmeClient(privateKeyHex, networkConfig);
```

#### Tokens
```csharp    
var someRemmePublicKey = newRemmeAccount.PublicKeyHex;
var balance = await client.Token.GetBalance(someRemmePublicKey);

var transactionResult = await  client.Token.Transfer(someRemmePublicKey, 100);

transactionResult.OnREMChainMessage += (sender, e) =>
{
  if (e.Status == BatchStatusEnum.COMMITTED)
  {
    var newBalance = await client.Token.GetBalance(someRemmePublicKey);
    transactionResult.CloseWebSocket();
  }
  else if (e.Status == BatchStatusEnum.NO_RESOURCE)
  {
    transactionResult.CloseWebSocket();
  }
};
transactionResult.ConnectToWebSocket();
```
#### Certificates/Public keys
```csharp
var userKeys = await client.PublicKeyStorage
                           .GetUserStoredPublicKeys(client.Account.PublicKeyHex);

var certificateTransactioResult = await client.Certificate
                                              .CreateAndStore(
                                                new CertificateCreateDto
	                                              {
                                                  CommonName = "userName1",
                                                  Email = "user@email.com",
                                                  Name = "John",
                                                  Surname = "Smith",
                                                  CountryName = "US",
                                                  ValidityDays = 360
                                                });

certificateTransactioResult.OnREMChainMessage += (sender, e) =>
{
  if (e.Status == BatchStatusEnum.COMMITTED)
  {
    var certX509 = certificateTransactioResult.CertificateDto.Certificate;
    var certPubKey = certificateTransactioResult.CertificateDto.PublicKeyPem;
		
    //Check the status of certificate public key
    var certificateStatus = await client.Certificate.Check(certX509);
		
    //It can be also done with RemmePublicKeyStorage
    var publicKeyCheckResult = await client.PublicKeyStorage.Check(certPubKey);
		
    // In this place additional logic can be stored. 
    // FI, saving user identifier to DataBase, or creating user account
		
    //Revoking certificate public key
    var revokeResult = await client.Certificate.Revoke(certX509).Result;
		
    certificateTransactioResult.CloseWebSocket();
  }	
  else if (e.Status == BatchStatusEnum.NO_RESOURCE)
  {
    certificateTransactioResult.CloseWebSocket();
  }
};
certificateTransactioResult.ConnectToWebSocket();
```
License
-------

REMME software and documentation are licensed under [Apache License Version 2.0](https://github.com/Remmeauth/remme-client-dotnet/blob/master/LICENCE).
