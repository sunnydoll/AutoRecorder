using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Json;
using Newtonsoft.Json;

namespace AutoRecorder
{
    class HttpHelper
    {
        private string connectURLPrefix = "https://api.phila.gov/opa/v1.1/account/";
        private string connectURLSuffix = "?format=json";

        public string addr = "";
        public string OPA = "";
        public Property prop;

        private string connectURLTax = "https://data.phila.gov/resource/y5ti-svsu.json?parcel_number=";

        public HttpHelper() { }
        public HttpHelper(string addr, string OPA)
        {
            this.addr = addr;
            this.OPA = OPA;
        }

        public void TaxCall()
        {
            Console.Out.WriteLine(groupTaxURL());
            WebRequest request = WebRequest.Create(groupTaxURL());
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            prop = new Property();
            //dynamic jsonObject = JsonObject.Parse(responseFromServer);
            //Console.WriteLine(jsonObject);
            //if (jsonObject != null && jsonObject.length > 0)
            //{

            //}
            TaxHistory[] hisArray = JsonConvert.DeserializeObject<TaxHistory[]>(responseFromServer);
            foreach(TaxHistory tax in hisArray) {
                prop.TaxOwed += Double.Parse(tax.total);
                Console.WriteLine(tax.total);
            }
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
                //prop.Owner = jsonObject["data"]["property"]["ownership"]["owners"].ToString();
                prop.Owner = string.Join(",", jsonObject["data"]["property"]["ownership"]["owners"]);
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
                prop.SalesPrice = Double.Parse(jsonObject["data"]["property"]["sales_information"]["sales_price"].ToString());
                string dateStr = jsonObject["data"]["property"]["sales_information"]["sales_date"].ToString();
                int start = dateStr.IndexOf('(') + 1;
                int end = dateStr.IndexOf('-');
                Console.WriteLine(dateStr.Substring(start, end - start));
                string dt = dateStr.Substring(start, end - start);
                long unixTime = 0;
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                if (dt != null && dt.Length > 0)
                {
                    unixTime = Int64.Parse(dateStr.Substring(start, end - start)) / 1000;
                    prop.SalesDate = epoch.AddSeconds(unixTime);
                }
                else
                {
                    prop.SalesDate = (DateTime?) null;
                }
                Console.WriteLine(epoch.AddSeconds(unixTime));
                //prop.SalesDate = Convert.ToDateTime(dateStr.Substring(start, end - start));
                prop.TaxOwed = Double.Parse(jsonObject["data"]["property"]["valuation_history"][0]["taxes"].ToString());
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

        private string groupTaxURL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(connectURLTax);
            sb.Append(OPA);
            Uri uri = new Uri(sb.ToString());
            return uri.AbsoluteUri;
        }
    }
}
