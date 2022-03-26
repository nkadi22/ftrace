using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Db
{
    public class MilkProduction
    {
        public int MilkProductionId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public float Quantity { get; set; }

        [NotMapped]
        public int PointInTime
        {
            get
            {
                return Year * 12 + Month;
            }
        }

        public int AnimalId { get; set; }
        //public Animal Animal { get; set; }
    }
}
