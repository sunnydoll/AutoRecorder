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
        private string connectURLPrefix = "https://api.phila.gov/opa/v1.1/address/";
        public string addr = "";
        private string connectURLSuffix = "/?format=json";
        public string OPA = "";

        public HttpHelper(string addr, string OPA)
        {
            this.addr = addr;
            this.OPA = OPA;
        }

        public string firstCall()
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
            JsonValue jsonObject = JsonObject.Parse(responseFromServer);
            // Display the content.
            if (jsonObject["data"]["properties"].Count > 0)
            {
                Console.WriteLine(jsonObject["data"]["properties"][0]["account_number"]);
            }

            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return "";
        }

        private string groupURL() {
            StringBuilder sb = new StringBuilder();
            sb.Append(connectURLPrefix);
            sb.Append(addr);
            sb.Append(connectURLSuffix);
            return Regex.Replace(sb.ToString(), @"\s+", "%20");
        }

    }
}
