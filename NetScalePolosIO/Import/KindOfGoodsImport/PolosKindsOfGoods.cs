using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO.KindOfGoodsImport
{
    public class KindOfGoodsImportRootObject
    {
        public List<KindOfGood> kindOfGoods { get; set; }
    }

    public class KindOfGood
    {
        public KindOfGood()
        {
            baseUnit = new BaseUnit();
        }

        public BaseUnit baseUnit { get; set; }

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