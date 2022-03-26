using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    public class MilkProductionDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public float Quantity { get; set; }

        public MilkProductionDto(MilkProduction milkProduction)
        {
            Year = milkProduction.Year;
            Month = milkProduction.Month;
            Quantity = milkProduction.Quantity;
        }
    }
}
