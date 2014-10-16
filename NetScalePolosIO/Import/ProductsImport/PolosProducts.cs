using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO.ProductsImport
{
    public class Product
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    public class ProduktRootObject
    {
        public List<Product> products { get; set; }
    }
}