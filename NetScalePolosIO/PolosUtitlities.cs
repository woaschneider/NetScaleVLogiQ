using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NetScalePolosIO
{
    public static class PolosUtitlities
    {
        public static DateTime ConvertPolosDateTime2DateTime(string s)
        {
            string format = "yyyyMMddHHmmssfff";
            string dateTime = s;
            DateTime dt = DateTime.ParseExact(dateTime, format, CultureInfo.InvariantCulture);
            return dt;
        }
    }
}