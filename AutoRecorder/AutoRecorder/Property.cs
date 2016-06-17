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
        public string MailingAddressCity = "";
        public string MailingAddressZipCode = "";
        public long LatestMarketValue = 0;
        public long ExemptLand = 0;
        public long ExemptImprovement = 0;
        public long HomesteadExemption = 0;
        public string Zoning = "";
        public long TotalTaxOwed = 0;
        public string TaxStatus = "";

        public Property() { }
    }
}
