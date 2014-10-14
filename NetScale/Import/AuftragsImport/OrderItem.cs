using System.Collections.Generic;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.AuftragsImport
{
    public class OrderItem
    {
        public string identifier { get; set; }
        public int sequence { get; set; }
        public string orderItemState { get; set; }
        public List<OrderItemService> orderItemServices { get; set; }
        public string plannedDate { get; set; }
        public Product product { get; set; }
    }
}