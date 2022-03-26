using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    /// <summary>
    /// Food usage data transfer object
    /// </summary>
    public class FoodUsageDto
    {
        /// <summary>
        /// Year
        /// </summary>
        /// <example>2022</example>
        public int Year { get; set; }
        /// <summary>
        /// Month
        /// </summary>
        /// <example>3</example>
        public int Month { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        /// <example>3</example>
        public float Quantity { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FoodUsageDto()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="foodUsage"></param>
        public FoodUsageDto(FoodUsage foodUsage)
        {
            Year = foodUsage.Year;
            Month = foodUsage.Month;
            Quantity = foodUsage.Quantity;
        }
    }
}
