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
            // OK
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            dynamic jsonObject = JsonObject.Parse(responseFromServer);
            prop = new Property();
            if (jsonObject.status.ToString().IndexOf("success") > -1)
            {
                //Console.WriteLine(jsonObject["data"]["property"]["ownership"]["owners"].GetType());
                //Console.WriteLine(jsonObject["data"]["property"]["ownership"]["owners"]);
                prop.Address = this.addr;
                prop.Street = jsonObject["data"]["property"]["full_address"].ToString();
                prop.OPA = this.OPA;
                //JsonArray jsonArr = new JsonArray(jsonObject["data"]["property"]["ownership"]["owners"]);
                //Console.WriteLine(jsonArr.ToString());
                prop.Owner = jsonObject["data"]["property"]["ownership"]["owners"].ToString();
                prop.MailingAddress = jsonObject["data"]["property"]["ownership"]["mailing_address"]["street"].ToString();
                prop.MailingAddressCity = jsonObject["data"]["property"]["ownership"]["mailing_address"]["city"].ToString();
                prop.MailingAddressZipCode = jsonObject["data"]["property"]["ownership"]["mailing_address"]["zip"].ToString();
                prop.LatestMarketValue = Int64.Parse(jsonObject["data"]["property"]["valuation_history"][0]["market_value"].ToString());
                prop.ExemptLand = Int64.Parse(jsonObject["data"]["property"]["valuation_history"][0]["land_exempt"].ToString());
                prop.ExemptImprovement = Int64.Parse(jsonObject["data"]["property"]["valuation_history"][0]["improvement_exempt"].ToString());
                if (jsonObject["data"]["property"]["characteristics"]["homestead"] != null)
                {
                    prop.HomesteadExemption = "Yes";
                }
                else
                {
                    prop.HomesteadExemption = "No";
                }
                prop.Zoning = jsonObject["data"]["property"]["characteristics"]["zoning"].ToString();
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
            Uri uri = new Uri(sb.ToString());
            return uri.AbsoluteUri;
        }

    }
}
