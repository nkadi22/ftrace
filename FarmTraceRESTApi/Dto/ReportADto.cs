using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    /// <summary>
    /// Total production of milk per animal type for a period
    /// </summary>
    public class ReportADto
    {
        /// <summary>
        /// Animal Type
        /// </summary>
        public string AnimalType { get; set; }
        /// <summary>
        /// Production
        /// </summary>
        public float Production { get; set; }
    }
}
