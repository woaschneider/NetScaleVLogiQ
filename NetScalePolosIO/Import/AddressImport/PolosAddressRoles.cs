using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HWB.NETSCALE.POLOSIO;

namespace NetScalePolosIO.Import.AddressImport
{
    
        public class Country
        {
            public int id { get; set; }
            public string isoCode { get; set; }
        }

        public class AddressRolesAddress
        {
            public string name { get; set; }
            public string street { get; set; }
            public string zipCode { get; set; }
            public string city { get; set; }
            public Country country { get; set; }
        }

        public class AddressRolesEntity
        {
            public int id { get; set; }
            public string businessIdentifier { get; set; }
            public string name { get; set; }
            public string owningLocationId { get; set; }
            public string subName { get; set; }
            public Address address { get; set; }
        }

        public class AddressRolesRootObject
        {
         
            public List<string> roles { get; set; }
        }
    }

