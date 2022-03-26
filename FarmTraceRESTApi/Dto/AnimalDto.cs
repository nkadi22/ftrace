using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    public class AnimalDto
    {
        public int AnimalId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Genre { get; set; }
        public List<FoodUsageDto> FoodUsages { get; set; }
        public List<MilkProductionDto> MilkProductions { get; set; }

        public AnimalDto(Animal animal, bool includeFoodUsage = true, bool includeProduction = true)
        {
            AnimalId = animal.AnimalId;
            Name = animal.Name;
            Type = animal.Type.ToString();
            Genre = animal.Genre.ToString();
            FoodUsages = new List<FoodUsageDto>();
            MilkProductions = new List<MilkProductionDto>();

            if (includeFoodUsage && animal.FoodUsages != null && animal.FoodUsages.Count > 0)
            {
                foreach (var foodUsage in animal.FoodUsages)
                {
                    FoodUsages.Add(new FoodUsageDto(foodUsage));
                }
            }

            if (includeProduction && animal.MilkProductions != null && animal.Genre == AnimalGenre.Female && animal.MilkProductions.Count > 0)
            {
                foreach (var milkProduction in animal.MilkProductions)
                {
                    MilkProductions.Add(new MilkProductionDto(milkProduction));
                }
            }
        }
    }
}
