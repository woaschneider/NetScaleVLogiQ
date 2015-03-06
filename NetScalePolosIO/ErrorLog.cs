using System;

using HWB.NETSCALE.BOEF;

namespace NetScalePolosIO
{
   public  class WriteErrorLog
   {
      
        // ErrorLog
        public void WriteToErrorLog(Exception e,WaegeEntity w)
        {
            ErrorLog oE = new ErrorLog();
            ErrorLogEntity oEe = oE.NewEntity();
            oEe.Dt = DateTime.Now;
            oEe.Message1 = e.Message;
            if (e.InnerException != null)
            {
                oEe.Message2 = e.InnerException.Message;
                oEe.Message3 = e.InnerException.Source;
            }
            if (w != null)
            {
                oEe.Lsnr = w.LieferscheinNr;
            }
            oE.SaveEntity(oEe);
        }
    }
}
