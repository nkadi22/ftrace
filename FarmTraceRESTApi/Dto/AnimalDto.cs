using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Dto
{
    /// <summary>
    /// Animal data transfer object
    /// </summary>
    public class AnimalDto
    {
        /// <summary>
        /// Animal Id
        /// </summary>
        /// <example>1</example>
        public int AnimalId { get; set; }
        /// <summary>
        /// Animal Name
        /// </summary>
        /// <example>Milky Cow</example>
        public string Name { get; set; }
        /// <summary>
        /// Animal Type
        /// </summary>
        /// <example>Cow</example>
        public string Type { get; set; }
        /// <summary>
        /// Animal Gender
        /// </summary>
        /// <example>1</example>
        public string Gender { get; set; }
        /// <summary>
        /// List of food consumption
        /// </summary>
        public List<FoodUsageDto> FoodUsages { get; set; }
        /// <summary>
        /// List of Milk Production
        /// </summary>
        public List<MilkProductionDto> MilkProductions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AnimalDto() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="animal"></param>
        /// <param name="includeFoodUsage"></param>
        /// <param name="includeProduction"></param>
        public AnimalDto(Animal animal, bool includeFoodUsage = true, bool includeProduction = true)
        {
            AnimalId = animal.AnimalId;
            Name = animal.Name;
            Type = animal.Type.ToString();
            Gender = animal.Gender.ToString();
            FoodUsages = new List<FoodUsageDto>();
            MilkProductions = new List<MilkProductionDto>();

            if (includeFoodUsage && animal.FoodUsages != null && animal.FoodUsages.Count > 0)
            {
                foreach (var foodUsage in animal.FoodUsages)
                {
                    FoodUsages.Add(new FoodUsageDto(foodUsage));
                }
            }

            if (includeProduction && animal.MilkProductions != null && animal.Gender == AnimalGenre.Female && animal.MilkProductions.Count > 0)
            {
                foreach (var milkProduction in animal.MilkProductions)
                {
                    MilkProductions.Add(new MilkProductionDto(milkProduction));
                }
            }
        }
    }
}
