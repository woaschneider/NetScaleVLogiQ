using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Controls;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.AuftragsImport
{
  public  class ImportAuftraege
    {

         public bool Import(string FullQualifiedFileName)
        {
            try
            {


                OrdersRootObject oOI = FullQualifiedFileName.CreateFromJsonFile<OrdersRootObject>();

             
               

               // foreach (orderItems obj in oOI.orderitemsEntities)
               //{
               //}
                
            }
             catch(Exception e)
             {
                 MessageBox.Show(e.Message.ToString());
             }
            return true;
        }
             
}
  }