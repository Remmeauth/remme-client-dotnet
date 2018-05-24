REMME .NET Client
==========
[![NuGet version](https://badge.fury.io/nu/https%3A%2F%2Fwww.nuget.org%2Fpackages%2FREMME.Auth.Client%2F0.1.0.svg)](https://badge.fury.io/nu/https%3A%2F%2Fwww.nuget.org%2Fpackages%2FREMME.Auth.Client%2F0.1.0)

**An open source .NET integration library for REMChain, simplifying the access and interaction with REMME nodes both public or permissioned.**

How to use
----------
1. Install and run REMME node with required REST API methods  enabled. 
You can check out how to do that at [REMME core repo](https://github.com/Remmeauth/remme-core/). 
*Note: you can enable/disable methods by modifying **REMME_REST_API_AVAILABLE_METHODS** eviroment variable at the .env file. *

2. Install the latest version of library to your .NET project
```
PM > Install-Package REMME.Auth.Client
```

3. Run methods of **RemmeClient** class to interract with REMME node. 

Examples
------------
#### Tokens
```csharp
	var client = new RemmeClient("localhost:8080", "localhost:9080");
    
	var someRemmeAddress = "0306796698d9b14a0ba313acc7f..";
    
	var balance = await client.Token.GetBalance(someRemmeAddress);
	
	var transactionResult = await client.Token.Transfer(someRemmeAddress, 100);
	transactionResult.BatchConfirmed += (sender, e) =>
	{
		var blockNumber =  e.BlockNumber;
		transactionResult.CloseWebSocket();
	};
	transactionResult.ConnectToWebSocket();

```
#### Certificates
```csharp
var certificateTransactioResult = await client
		Certificate
		.CreateAndStoreCertificate(
		new CertificateCreateDto
			{
				CommonName = "userName1",
				Email = "user@email.com",
				Name = "John",
				Surname = "Smith",
				CountryName = "US",
				Validity = 360
			});
certificateTransactioResult.BatchConfirmed += (sender, e) =>
{
	var certificateStatus = client
		.Certificate
		.CheckCertificate(certificateTransactioResult.Certificate).Result;

	 // In this place additional logic can be stored. 
	// FI, saving user identifier to DataBase, or creating user account
	certificateTransactioResult.CloseWebSocket();
};
certificateTransactioResult.ConnectToWebSocket();
```



License
-------

REMME software and documentation are licensed under `Apache License Version 2.0 <LICENCE>`_.





