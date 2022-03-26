using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    public class ReportADto
    {
        public string AnimalType { get; set; }
        public float Production { get; set; }

        /*public ReportADto(AnimalType AnimalType, float TotalProduction)
        {
            this.AnimalType = AnimalType;
            this.TotalProduction = TotalProduction;
        }*/
    }
}
