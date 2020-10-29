using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldWeb.Models
{
    public class AddressVM
    {
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public int PostalCode { get; set; }
        public string Country { get; set; }
    }
}
