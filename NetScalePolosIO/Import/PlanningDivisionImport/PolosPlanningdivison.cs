using System.Collections.Generic;

namespace HWB.NETSCALE.POLOSIO.PlanningDivisionImport
{
    public class PolosPlanningdivison
    {
    public string id { get; set; }
    public string description { get; set; }
    public bool active { get; set; }

    }


    public class PlanningDivisonRootObject
    {
        public List<PolosPlanningdivison> planningdivisons{ get; set; }
    }
}