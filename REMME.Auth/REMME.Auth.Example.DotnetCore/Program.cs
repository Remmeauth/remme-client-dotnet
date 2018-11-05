using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Implementation;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.Batch;
using System;
using System.Linq;

namespace REMME.Auth.DotnetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Address of Docker container
            string nodeAddress = "192.168.99.100";

            //Private Key from REMChain account
            var privateKeyHex = "78a8f39be4570ba8dbb9b87e6918a4c2559bc4e8f3206a0a755c6f2b659a7850";

            //Initialize client
            var client = new RemmeClient(privateKeyHex, new RemmeNetworkConfig { NodeAddress = nodeAddress });

            //Account operations
            var newRemmeAccount = new RemmeAccount();
            Console.WriteLine("There was created new KeyPair for account with {0} Public Key", newRemmeAccount.PublicKeyHex);

            //Token Operations
            var someRemmePublicKey = newRemmeAccount.PublicKeyHex;
            var balance = client.Token.GetBalance(someRemmePublicKey).Result;
            Console.WriteLine("Account {0} balance - {1} REM", someRemmePublicKey, balance);
            var transactionResult = client.Token.Transfer(someRemmePublicKey, 100).Result;
            Console.WriteLine("Sending tokens...BatchId: {0}", transactionResult.BatchId);

            transactionResult.OnREMChainMessage += (sender, e) =>
            {
                if (e.Status == BatchStatusEnum.COMMITTED)
                {
                    Console.WriteLine("Tokens were sent");

                    var newBalance = client.Token.GetBalance(someRemmePublicKey).Result;
                    Console.WriteLine("Account {0} balance - {1} REM", someRemmePublicKey, newBalance);

                    transactionResult.CloseWebSocket();
                }
                else if (e.Status == BatchStatusEnum.NO_RESOURCE)
                {
                    transactionResult.CloseWebSocket();
                }
            };
            transactionResult.ConnectToWebSocket();


            //Certificates/PubKeys Operations
            var userKeys = client.PublicKeyStorage.GetAccountStoredPublicKeys(client.Account.PublicKeyHex).Result;
            Console.WriteLine("User has {0} stored public keys", userKeys.Count());

            var certificateTransactioResult = client
                                                .Certificate
                                                .CreateAndStore(
                                                    new CertificateCreateDto
                                                    {
                                                        CommonName = "userName1",
                                                        Email = "user@email.com",
                                                        Name = "John",
                                                        Surname = "Smith",
                                                        CountryName = "US",
                                                        ValidityDays = 360
                                                    }).Result;

            Console.WriteLine("Issuing certificate... And storing public key on REMChain BatchId: ",
                    certificateTransactioResult.BatchId);

            certificateTransactioResult.OnREMChainMessage += (sender, e) =>
            {
                if (e.Status == BatchStatusEnum.COMMITTED)
                {
                    var certX509 = certificateTransactioResult.CertificateDto.Certificate;
                    var certPubKey = certificateTransactioResult.CertificateDto.PublicKeyPem;

                    Console.WriteLine("Certificate public key was saved on REMchain");

                    //Check the status of certificate public key
                    var certificateStatus = client
                            .Certificate
                            .Check(certX509).Result;

                    Console.WriteLine("Certificate IsValid = {0}", certificateStatus.IsValid);

                    //It can be also done with RemmePublicKeyStorage
                    var publicKeyCheckResult = client.PublicKeyStorage.Check(certPubKey).Result;
                    Console.WriteLine("Certificate IsValid = {0}", publicKeyCheckResult.IsValid);

                    // In this place additional logic can be stored. 
                    // FI, saving user identifier to DataBase, or creating user account

                    //Revoking certificate public key
                    var revokeResult = client
                                        .Certificate
                                        .Revoke(certX509).Result;

                    certificateTransactioResult.CloseWebSocket();
                }
                else if (e.Status == BatchStatusEnum.NO_RESOURCE)
                {
                    certificateTransactioResult.CloseWebSocket();
                }
            };

            certificateTransactioResult.ConnectToWebSocket();

            Console.ReadKey(true);
        }
    }
}
