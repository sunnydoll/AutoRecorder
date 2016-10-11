using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRecorder
{
    class Property
    {
        public string Address = "";
        public string Street = "";
        public string OPA = ""; //account number
        public string Owner = "";
        public string MailingAddress = "";
        public string MailingAddressCity = "";
        public string MailingAddressZipCode = "";
        public long LatestMarketValue = 0;
        public long ExemptLand = 0;
        public long ExemptImprovement = 0;
        public string HomesteadExemption = "";
        public string Zoning = "";
        public double SalesPrice = 0.0;
        public DateTime? SalesDate = null;
        public double TaxOwed = 0.0;

        public Property() { }
    }
}
