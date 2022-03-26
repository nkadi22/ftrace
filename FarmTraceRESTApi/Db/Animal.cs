using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Db
{
    public class Animal
    {
        public int AnimalId { get; set; }
        public string Name { get; set; }
        public AnimalType Type { get; set; }
        public AnimalGenre Genre { get; set; }
        /*public float MilkProduced { get; set; }
        public float FoodQuantity { get; set; }*/

        public List<FoodUsage> FoodUsages { get; set; }
        public List<MilkProduction> MilkProductions { get; set; }
    }

    public enum AnimalType
    {
        Cow = 0,
        Goat
    }

    public enum AnimalGenre
    {
        Male = 0,
        Female
    }
}
