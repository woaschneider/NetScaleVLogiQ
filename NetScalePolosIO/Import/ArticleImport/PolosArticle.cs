using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO.ArticleImport
{
    public class ArticleRootObject
    {
        public List<ArticleInformation> articleInformation { get; set; }
    }

    public class ArticleInformation
    {
        public Article article { get; set; }
        public List<string> attributes { get; set; }
    }

    public class Article
    {
        public Article()
        {
            var baseUnit = new BaseUnit();
            var conversionUnit = new ConversionUnit();
        }

        public string id { get; set; }
        public int ownerId { get; set; }
        public string locationId { get; set; }
        public string number { get; set; }
        public string kindOfGoodDescription { get; set; }
        public int kindOfGoodId { get; set; }
        public string description { get; set; }
        public BaseUnit baseUnit { get; set; }
        public ConversionUnit conversionUnit { get; set; }
    }

    public class BaseUnit
    {
        public int id { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
    }

    public class ConversionUnit
    {
        public int id { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
    }
}