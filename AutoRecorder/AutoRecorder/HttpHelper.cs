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
        private string connectURLSuffix = "?format=json";

        public string addr = "";
        public string OPA = "";
        public Property prop;

        private string connectURLTax = "http://www.phila.gov/revenue/realestatetax/";
        public string txtTaxInfo = "";
        public string hcBrtNum = "";
        private string VIEWSTATE = "/wEPDwULLTEyNDQ4MDU4OTkPZBYCZg9kFgICAw9kFgICDQ9kFgYCAQ9kFgICAw9kFgICAQ8QZGQWAGQCBQ8PFgIeBFRleHRlZGQCDQ9kFgYCAQ88KwAKAGQCBQ8UKwACZBAWABYAFgBkAgcPPCsAEQEBEBYAFgAWAGQYAgVBY3RsMDAkQm9keUNvbnRlbnRQbGFjZUhvbGRlciRHZXRUYXhJbmZvQ29udHJvbCRncmRQYXltZW50c0hpc3RvcnkPZ2QFMmN0bDAwJEJvZHlDb250ZW50UGxhY2VIb2xkZXIkR2V0VGF4SW5mb0NvbnRyb2wkZnJtD2dkphVyS0zNWc/1VgImp+wXiI68igV1WHYoH/5TFDo03cY=";
        private string EVENTVALIDATION = "/wEWBQLIx7vqBQLRzsWTBwLlpIbACAKV6q2KDQKIvdHyCf4VKLOQVu5fnj0I/4w0y8mRx5YMYkECgNfsVzn4fw6U";

        public HttpHelper() { }
        public HttpHelper(string addr, string OPA)
        {
            this.addr = addr;
            this.OPA = OPA;
        }

        public void TaxCall()
        {
            WebRequest request = WebRequest.Create(connectURLTax);
            request.Method = "POST";
            string postJson = "{\"ctl00$BodyContentPlaceHolder$SearchByBRTControl$txtTaxInfo\":\""+ OPA + "\"," +
                "\"__VIEWSTATE\":\"" + VIEWSTATE + "\"," +
                "\"__EVENTVALIDATION\":\"" + EVENTVALIDATION + "\"," +
                "\"ctl00$BodyContentPlaceHolder$SearchByBRTControl$btnTaxByBRT\":\" >>\"}";
            //Console.WriteLine(postJson);
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            byte[] byteArray = Encoding.UTF8.GetBytes(postJson);
            //request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
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
