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
            string nodeAddress = "192.168.99.101:8080";
            string socketAddress = "192.168.99.101:9080";

            //Initialize client
            var client = new RemmeClient(nodeAddress, socketAddress);

            //Token Operations
            var someRemmeAddress = "0306796698d9b14a0ba313acc7fb14f69d8717393af5b02cc292d72009b97d8759";
            var balance = client.Token.GetBalance(someRemmeAddress).Result;
            Console.WriteLine("Account {0} balance - {1} REM", someRemmeAddress, balance);

            var transactionResult = client.Token.Transfer(someRemmeAddress, 100).Result;
            Console.WriteLine("Sending tokens...BatchId: {0}", transactionResult.BatchId);

            transactionResult.BatchConfirmed += (sender, e) =>
            {
                Console.WriteLine("Tokens were sent at block number: ", e.BlockNumber);

                var newBalance = client.Token.GetBalance(someRemmeAddress).Result;
                Console.WriteLine("Account {0} balance - {1} REM", someRemmeAddress, newBalance);
                transactionResult.CloseWebSocket();
            };

            transactionResult.ConnectToWebSocket();

            
            //Certificates Operations

            var certificateTransactioResult = client
                                                .Certificate
                                                .CreateAndStoreCertificate(
                                                    new CertificateCreateDto
                                                    {
                                                        CommonName = "userName1",
                                                        Email = "user@email.com",
                                                        Name = "John",
                                                        Surname = "Smith",
                                                        CountryName = "US",                
                                                        Validity = 360
                                                    }).Result;
            Console.WriteLine("Issuing certificate... BatchId: ", certificateTransactioResult.BatchId);
            certificateTransactioResult.BatchConfirmed += (sender, e) =>
            {
                Console.WriteLine("Certificate was saved on REMchain at block number: ", e.BlockNumber);

                var certificateStatus = client
                                            .Certificate
                                            .CheckCertificate(certificateTransactioResult.Certificate).Result;
                Console.WriteLine("Certificate IsValid = {0}", certificateStatus);

                // In this place additional logic can be stored. 
                // FI, saving user identifier to DataBase, or creating user account

                certificateTransactioResult.CloseWebSocket();
            };
            certificateTransactioResult.ConnectToWebSocket();

            Console.ReadKey(true);
        }
    }
}
