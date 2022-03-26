using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    public class Period
    {
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }

        public int StartPointInTime
        {
            get
            {
                return StartYear * 12 + StartMonth;
            }
        }

        public int EndPointInTime
        {
            get
            {
                return EndYear * 12 + EndMonth;
            }
        }

        public bool IsValid()
        {
            if (StartYear > EndYear)
            {
                return false;
            }

            if (StartYear == EndYear)
            {
                return StartMonth <= EndMonth;
            }

            return true;
        }
    }
}
