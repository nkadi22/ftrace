using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    public class FoodUsageDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public float Quantity { get; set; }

        public FoodUsageDto(FoodUsage foodUsage)
        {
            Year = foodUsage.Year;
            Month = foodUsage.Month;
            Quantity = foodUsage.Quantity;
        }
    }
}
