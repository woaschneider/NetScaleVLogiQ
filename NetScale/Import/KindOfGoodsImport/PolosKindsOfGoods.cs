using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.KindOfGoodsImport
{
    public class RootObject
    {
        public List<KindOfGood> kindOfGoods { get; set; }
    }

    public class KindOfGood
    {
        public BaseUnit baseUnit { get; set; }
        public KindOfGood()
        {
            baseUnit = new BaseUnit();
        }

        public string id { get; set; }
        public string description { get; set; }
        
    }

    public class BaseUnit
    {
        public int id { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
    }

  

   
}
