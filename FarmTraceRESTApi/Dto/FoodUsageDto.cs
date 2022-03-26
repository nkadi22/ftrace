using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
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

        public FoodUsageDto()
        {
            
        }

        public FoodUsageDto(FoodUsage foodUsage)
        {
            Year = foodUsage.Year;
            Month = foodUsage.Month;
            Quantity = foodUsage.Quantity;
        }
    }
}
