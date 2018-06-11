using System;
using REMME.Auth.Client.Implementation;
using REMME.Auth.Client.Contracts.Models;

namespace REMME.Auth.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Addresses of Docker container ports
            string nodeAddress = "192.168.99.100:8080";
            string socketAddress = "192.168.99.100:9080";

            var privateKeyHex = "78a8f39be4570ba8dbb9b87e6918a4c2559bc4e8f3206a0a755c6f2b659a7850";

            //Initialize client
            var client = new RemmeClient(privateKeyHex, nodeAddress, socketAddress);

            //Token Operations
            var someRemmeAddress = "0306796698d9b14a0ba313acc7fb14f69d8717393af5b02cc292d72009b97d8759";
            var balance = client.Token.GetBalance(someRemmeAddress).Result;
            Console.WriteLine("Account {0} balance - {1} REM", someRemmeAddress, balance);

            var transactionResult = client.Token.Transfer(someRemmeAddress, 100).Result;
            Console.WriteLine("Sending tokens...BatchId: {0}", transactionResult.BatchId);

            transactionResult.OnREMChainMessage += (sender, e) =>
            {
                if(e.Status == Client.RemmeApi.Models.BatchStatusEnum.OK)
                {
                    Console.WriteLine("Tokens were sent");

                    var newBalance = client.Token.GetBalance(someRemmeAddress).Result;
                    Console.WriteLine("Account {0} balance - {1} REM", someRemmeAddress, newBalance);
                }
                transactionResult.CloseWebSocket();
            };

            transactionResult.ConnectToWebSocket();

            
            //Certificates Operations

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
            Console.WriteLine("Issuing certificate... BatchId: ", certificateTransactioResult.BatchId);
            certificateTransactioResult.OnREMChainMessage += (sender, e) =>
            {
                if(e.Status == Client.RemmeApi.Models.BatchStatusEnum.OK)
                {
                    Console.WriteLine("Certificate was saved on REMchain");

                    var certificateStatus = client
                            .Certificate
                            .CheckCertificate(certificateTransactioResult.CertificateDto.Certificate).Result;
                    Console.WriteLine("Certificate IsValid = {0}", certificateStatus);

                    // In this place additional logic can be stored. 
                    // FI, saving user identifier to DataBase, or creating user account
                }


                certificateTransactioResult.CloseWebSocket();
            };
            certificateTransactioResult.ConnectToWebSocket();

            Console.ReadKey(true);
        }
    }
}
