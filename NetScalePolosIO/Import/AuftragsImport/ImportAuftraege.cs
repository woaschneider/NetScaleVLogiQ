using System;
using HWB.NETSCALE.BOEF;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.AuftragsImport
{
    public class ImportAuftraege
    {
        private Orderitem boO = new Orderitem();
        private OrderitemEntity boOE;
        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                var oOI = FullQualifiedFileName.CreateFromJsonFile<RootObject>();

                

                 foreach (OrderEntity obj in oOI.orderEntities)
                {
                    if(obj.id!=null)
                    {
                        boOE = boO.GetById(obj.id);
                    }
                     if(boOE==null)
                     {
                       boOE=  boO.NewEntity();
                     }
                     if(boOE!=null)
                     {
                         boOE.id = obj.id;
                         boOE.locationId = obj.locationId;
                         boOE.number = obj.number;
                      //   boOE.date = Convert.ToDateTime( obj.date);
                      boOE.orderstate = obj.orderState;
                         boO.SaveEntity(boOE);
                     }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return true;
        }
    }
}