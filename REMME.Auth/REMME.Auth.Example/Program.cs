using REMME.Auth.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace REMME.Auth.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var client = new RemmeClient("192.168.99.101:8080");
                var cert = client.CreateCertificate("tolik", "email@etst.t").Result;
                var recheck = client.CheckCertificate(cert).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception: {ex.ToString()}");
            }        
        }
    }
}
