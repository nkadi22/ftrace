using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    /// <summary>
    /// Total amount of food used per animal type
    /// </summary>
    public class ReportBDto
    {
        /// <summary>
        /// Animal Type
        /// </summary>
        public string AnimalType { get; set; }
        /// <summary>
        /// Food Usage
        /// </summary>
        public float FoodUsage { get; set; }
    }
}
