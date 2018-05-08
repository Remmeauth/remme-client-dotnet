using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace REMME.Auth.Client.RemmeApi
{
    public class RemmeRest
    {
        public async Task<Output> PutRequest<Input, Output>(string url, Input requestPayload)
        {
            Output result = default(Output);
            using (var client = new HttpClient())
            {
                var stringPayload = JsonConvert.SerializeObject(requestPayload);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PutAsync(url, httpContent);
                    var str = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<Output>(str);
                }
                catch (Exception ex)
                {

                }
            }

            return result;
        }
    }
}
