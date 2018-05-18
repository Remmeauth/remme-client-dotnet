using REMME.Auth.Client;
using REMME.Auth.Client.RemmeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace REMME.Auth.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RemmeClient("192.168.99.101:8080");
            var testResult = client.CreateCertificate("testName", "email@etst.t").Result;
            testResult.BatchConfirmed += (sender, e) => Console.WriteLine("Batch confirmed " + e.Data);
            testResult.ConnectToWebSocket();
            Console.ReadKey(true); 
        }
    }
}
