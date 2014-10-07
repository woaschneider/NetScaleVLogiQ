using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HWB.NETSCALE.BOEF;
using Microsoft.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace HWB.NETSCALE.FRONTEND.WPF.Import
{
    public class PolosAddress : Object
    {

        public class Country
        {
            public int id { get; set; }
            public string isoCode { get; set; }
        }

        public class Address
        {
            public string name { get; set; }
            public string street { get; set; }
            public string zipCode { get; set; }
            public string city { get; set; }
            public Country country { get; set; }
        }

        public class AddressableEntity
        {
            public int id { get; set; }
            public string businessIdentifier { get; set; }
            public string name { get; set; }
            public string owningLocationId { get; set; }
            public string subName { get; set; }
            public Address address { get; set; }
            public string subName2 { get; set; }
        }

        public class RootObject
        {
            public List<AddressableEntity> addressableEntities { get; set; }
        }

    }
}