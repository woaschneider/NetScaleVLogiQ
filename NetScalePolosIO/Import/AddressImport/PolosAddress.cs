using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO
{
    public class AddressRootObject
    {
        public List<AddressableEntity> addressableEntities { get; set; }
    }

    public class AddressableEntity
    {
        public AddressableEntity()
        {
            address = new Address();
        }

        public Address address { get; set; }

        public int id { get; set; }
        public string businessIdentifier { get; set; }
        public string name { get; set; }
        public string owningLocationId { get; set; }
        public string subName { get; set; }

        public string subName2 { get; set; }
    }


    public class Address
    {
        public Country country;

        public Address()
        {
            country = new Country();
        }

        public string name { get; set; }
        public string street { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
    }

    public class Country
    {
        public int id { get; set; }
        public string isoCode { get; set; }
    }
}