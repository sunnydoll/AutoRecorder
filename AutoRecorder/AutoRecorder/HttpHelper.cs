using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Json;

namespace AutoRecorder
{
    class HttpHelper
    {
        private string connectURLPrefix = "https://api.phila.gov/opa/v1.1/account/";
        public string addr = "";
        private string connectURLSuffix = "?format=json";
        public string OPA = "";
        public Property prop;

        public HttpHelper() { }
        public HttpHelper(string addr, string OPA)
        {
            this.addr = addr;
            this.OPA = OPA;
        }

        public void OPACall()
        {
            string connectURL = groupURL();
            Console.Out.WriteLine(connectURL); 
            WebRequest request = WebRequest.Create(connectURL);
            //request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            dynamic jsonObject = JsonObject.Parse(responseFromServer);
            Console.WriteLine(jsonObject.status.ToString());
            // Display the content.
            if (jsonObject.status.ToString() != "error" && jsonObject["data"]["properties"].Count == 1)
            {
                Console.WriteLine(jsonObject["data"]["properties"][0]["account_number"]);
                prop = new Property();
                prop.Address = this.addr;
                prop.Street = jsonObject["data"]["properties"][0]["address_match"]["standardized"].ToString();
                prop.OPA = this.OPA;
                prop.LatestMarketValue = Int64.Parse(jsonObject["data"]["properties"][0]["valuation_history"][0]["market_value"].ToString());
                prop.ExemptLand = Int64.Parse(jsonObject["data"]["properties"][0]["valuation_history"][0]["market_value"].ToString());
                prop.ExemptImprovement = Int64.Parse(jsonObject["data"]["properties"][0]["valuation_history"][0]["improvement_exempt"].ToString());
                if (jsonObject["data"]["properties"][0]["characteristics"]["homestead"] != null)
                {
                    prop.HomesteadExemption = "Yes";
                }
                else
                {
                    prop.HomesteadExemption = "No";
                }
                prop.Zoning = jsonObject["data"]["properties"][0]["characteristics"]["zoning"].ToString();
            }

            // Clean up the streams and the response.
            reader.Close();
            response.Close();
        }

        private string groupURL() {
            StringBuilder sb = new StringBuilder();
            sb.Append(connectURLPrefix);
            sb.Append(this.OPA);
            sb.Append(connectURLSuffix);
            return Regex.Replace(sb.ToString(), @"\s+", "%20");
        }

    }
}
