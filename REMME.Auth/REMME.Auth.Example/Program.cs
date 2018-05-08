using REMME.Auth.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
                new RemmeClient().CreateCertificate().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception: {ex.ToString()}");
            }


        }
    }
}
