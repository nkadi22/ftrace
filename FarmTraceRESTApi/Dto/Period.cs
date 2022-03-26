using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    /// <summary>
    /// Period of time
    /// </summary>
    public class Period
    {
        /// <summary>
        /// Year part of the period's starting point
        /// </summary>
        /// <example>2022</example>
        public int StartYear { get; set; }
        /// <summary>
        /// Month part of the period's starting point
        /// </summary>
        /// <example>1</example>
        public int StartMonth { get; set; }
        /// <summary>
        /// Year part of the period's end point
        /// </summary>
        /// <example>2022</example>
        public int EndYear { get; set; }
        /// <summary>
        /// Month part of the period's end point
        /// </summary>
        /// <example>3</example>
        public int EndMonth { get; set; }

        /// <summary>
        /// Start point in time represented in months
        /// </summary>
        public int StartPointInTime
        {
            get
            {
                return StartYear * 12 + StartMonth;
            }
        }

        /// <summary>
        /// End point in time represented in months
        /// </summary>
        public int EndPointInTime
        {
            get
            {
                return EndYear * 12 + EndMonth;
            }
        }

        /// <summary>
        /// True if the period of time is valid, false otherwise
        /// </summary>
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
