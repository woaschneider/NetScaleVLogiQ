using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.ProductsImport
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
