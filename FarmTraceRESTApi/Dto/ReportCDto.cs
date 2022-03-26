using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    /// <summary>
    /// Best producing animals
    /// </summary>
    public class ReportCDto
    {
        /// <summary>
        /// Animal Name
        /// </summary>
        public string AnimalName { get; set; }
        /// <summary>
        /// Production
        /// </summary>
        public float Production { get; set; }
    }
}
