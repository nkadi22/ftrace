using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    public class MilkProductionDto
    {
        /// <summary>
        /// Year
        /// </summary>
        /// <example>2022</example>
        public int Year { get; set; }
        /// <summary>
        /// Month
        /// </summary>
        /// <example>2</example>
        public int Month { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        /// <example>2.5</example>
        public float Quantity { get; set; }

        public MilkProductionDto() { }

        public MilkProductionDto(MilkProduction milkProduction)
        {
            Year = milkProduction.Year;
            Month = milkProduction.Month;
            Quantity = milkProduction.Quantity;
        }
    }
}
