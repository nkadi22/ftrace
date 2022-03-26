using FarmTraceWebServer.Core;
using FarmTraceWebServer.Db;
using FarmTraceWebServer.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FarmTraceController : ControllerBase
    {
        private readonly ILogger<FarmTraceController> _logger;
        private readonly IAnimalData _animalData;
        private readonly IFarmTraceDatabaseContext _dbContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger object.</param>
        /// <param name="animalData">Object that keeps animal data (read from the input file) in memory.</param>
        /// <param name="dbContext">EF Database context for the app.</param>
        /// <returns></returns>
        public FarmTraceController(ILogger<FarmTraceController> logger, IAnimalData animalData, IFarmTraceDatabaseContext dbContext)
        {
            _logger = logger;
            _animalData = animalData;
            _animalData.Init();
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all existing animal data
        /// </summary>
        /// <param name="imported">True indicates that data will be read from database. False indicates that data will be based on the input file 'data.json'.</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Animal data was sucessufully retrieved", Type = typeof(AnimalDto))]
        [HttpGet]
        public IEnumerable<AnimalDto> Get(bool imported = false)
        {
            List<Animal> list = imported ? _dbContext.Animals.Include("FoodUsages").Include("MilkProductions").ToList() : ((dynamic)putIdsOnInMemoryData()).Animals;
            List<AnimalDto> ret = new List<AnimalDto>();

            if (list != null)
            {
                foreach (var animal in list)
                {
                    ret.Add(new AnimalDto(animal));
                }
            }

            return ret;
        }

        /// <summary>
        /// Retrieves data from a specific animal
        /// </summary>
        /// <param name="id">The Animal Id.</param>
        /// <param name="imported">Indicates the source of the data: true -> database, false -> file 'data.json'</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Animal data was sucessufully retrieved", Type = typeof(AnimalDto))]
        [SwaggerResponse((int)HttpStatusCode.NotFound,
            "Animal was not found on the database")]
        [HttpGet("{id}")]
        public ActionResult<Animal> GetSpecificAnimalDataFromDB(int id, bool imported = false)
        {
            dynamic memoryData = putIdsOnInMemoryData();
            List<Animal> animalsFromFile = memoryData.Animals;
            Animal animal = imported ? _dbContext.Animals.Include("FoodUsages").Include("MilkProductions").FirstOrDefault(row => row.AnimalId == id)
                                        : System.Linq.Enumerable.FirstOrDefault(animalsFromFile, a => a.AnimalId == id);
            
            if (animal != null)
            {
                var ret = new AnimalDto(animal);
                return Ok(ret);
            }

            return NotFound();
        }

        /// <summary>
        /// Retrieves data from a specific animal by name
        /// </summary>
        /// <param name="name">The Animal name.</param>
        /// <param name="imported">True indicates that data will be read from database. False indicates that data will be based on the input file 'data.json'.</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Animal data was sucessufully retrieved", Type = typeof(AnimalDto))]
        [SwaggerResponse((int)HttpStatusCode.NotFound,
            "Animal was not found on the database")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest,
            "Animal name not provided.")]
        [HttpGet("getanimalbyname")]
        public ActionResult<Animal> GetSpecificAnimalDataDataByName(string name, bool imported = false)
        {
            if (name == null)
            {
                return BadRequest("Malformed Request.");
            }

            Animal animal = imported ? _dbContext.Animals.Include("FoodUsages").Include("MilkProductions").FirstOrDefault(row => row.Name == name)
                                : System.Linq.Enumerable.FirstOrDefault(_animalData.SelectAllAnimals(), a => a.Name.Equals(name));

            if (animal != null)
            {
                var ret = new AnimalDto(animal);
                return Ok(ret);
            }

            return NotFound();
        }

        /// <summary>
        /// Deletes a specific Animal by Id (from database only).
        /// </summary>
        /// <param name="id">The Animal id</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.NotFound,
            "Animal does not exist")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError,
            "Error while deleting the Animal data")]
        [SwaggerResponse((int)HttpStatusCode.NoContent,
            "Animal deleted successfully")]
        [HttpDelete("{id}")]
        public ActionResult DeleteAnimal(int id)
        {
            Animal animal = _dbContext.Animals.Find(id);

            if (animal == null)
            {
                return NotFound();
            }

            _dbContext.Animals.Remove(animal);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + "\n" + e.InnerException);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// ReportA: Total production of milk per animal type for a period.
        /// </summary>
        /// <param name="imported">Indicates the source of the data: true -> database, false -> file 'data.json'</param>
        /// <param name="value">Time interval of the report</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.BadRequest,
            "Time interval is not valid")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Report has been successfully generated")]
        [HttpGet("ReportA")]
        public ActionResult<List<ReportADto>> GetTotalProductionPerAnimal(bool imported, [FromBody] Period value)
        {
            if (value == null || !value.IsValid())
            {
                return BadRequest("Malformed Request.");
            }

            List<Animal> animals;
            List<MilkProduction> milkProductions;

            if (imported)
            {
                animals = _dbContext.Animals.ToList();
                milkProductions = _dbContext.MilkProductions.ToList();
            }
            else
            {
                dynamic inMemoryData = putIdsOnInMemoryData();
                animals = inMemoryData.Animals;
                milkProductions = inMemoryData.MilkProductions;
            }

            List<ReportADto> ret = new List<ReportADto>();
            if (animals != null && milkProductions != null)
            {
                var productions = from a in animals
                                  from p in milkProductions
                                  where a.AnimalId == p.AnimalId &&
                                       (p.Year * 12 + p.Month) >= (value.StartYear * 12 + value.StartMonth) &&
                                       (p.Year * 12 + p.Month) <= (value.EndYear * 12 + value.EndMonth)
                                  select new ReportADto() { AnimalType = a.Type.ToString(), Production = p.Quantity };

                var groupingProductions = from p in productions
                                          group p by p.AnimalType into newGroup
                                          select new ReportADto() { AnimalType = newGroup.Key, Production = newGroup.Sum(x => x.Production) };

                ret = groupingProductions.ToList();
            }

            return Ok(ret);
        }

        /// <summary>
        /// ReportB: Total amount of food used per animal type.
        /// </summary>
        /// <param name="imported">Indicates the source of the data: true -> database, false -> file 'data.json'</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Report has been successfully generated")]
        [HttpGet("ReportB")]
        public ActionResult<List<ReportBDto>> GetTotalAmountOfFoodPerAnimal(bool imported)
        {
            List<Animal> animals;
            List<FoodUsage> foodUsages;

            if (imported)
            {
                animals = _dbContext.Animals.ToList();
                foodUsages = _dbContext.FoodUsages.ToList();
            }
            else
            {
                dynamic inMemoryData = putIdsOnInMemoryData();
                animals = inMemoryData.Animals;
                foodUsages = inMemoryData.FoodUsages;
            }

            List<ReportBDto> ret = new List<ReportBDto>();
            if (animals != null && foodUsages != null)
            {
                var usageOfFood = from a in animals
                                  from p in foodUsages
                                  where a.AnimalId == p.AnimalId
                                  select new ReportBDto() { AnimalType = a.Type.ToString(), FoodUsage = p.Quantity };

                var groupingUsages = from u in usageOfFood
                                     group u by u.AnimalType into newGroup
                                     select new ReportBDto() { AnimalType = newGroup.Key, FoodUsage = newGroup.Sum(x => x.FoodUsage) };

                ret = groupingUsages.ToList();
            }

            return Ok(ret);
        }

        /// <summary>
        /// ReportC: Best producing animals.
        /// </summary>
        /// <param name="imported">Indicates the source of the data: true -> database, false -> file 'data.json'</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Report has been successfully generated")]
        [HttpGet("ReportC")]
        public ActionResult<List<ReportCDto>> GetBestProducingAnimals(bool imported)
        {
            List<Animal> animals;
            List<MilkProduction> milkProductions;

            if (imported)
            {
                animals = _dbContext.Animals.ToList();
                milkProductions = _dbContext.MilkProductions.ToList();
            }
            else
            {
                dynamic inMemoryData = putIdsOnInMemoryData();
                animals = inMemoryData.Animals;
                milkProductions = inMemoryData.MilkProductions;
            }

            List<ReportCDto> ret = new List<ReportCDto>();
            if (animals != null && milkProductions != null)
            {
                var productions = from a in animals
                                  from p in milkProductions
                                  where a.AnimalId == p.AnimalId
                                  select new ReportCDto() { AnimalName = a.Name, Production = p.Quantity };

                var groupingProductions = from p in productions
                                          group p by p.AnimalName into newGroup
                                          select new ReportCDto() { AnimalName = newGroup.Key, Production = newGroup.Sum(x => x.Production) };

                var orderingBy = from p in groupingProductions
                                 orderby p.Production descending
                                 select p;

                ret = orderingBy.ToList();
            }

            return Ok(ret);
        }

        /// <summary>
        /// ReportD: Average production for a period.
        /// </summary>
        /// <param name="imported">Indicates the source of the data: true -> database, false -> file 'data.json'</param>
        /// <param name="value">Time interval of the report</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.BadRequest,
            "Time interval is not valid")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Report has been successfully generated")]
        [HttpGet("ReportD")]
        public ActionResult<ReportDDto> GetAverageProductionForAPeriod(bool imported, [FromBody] Period value)
        {
            if (value == null || !value.IsValid())
            {
                return BadRequest("Malformed Request.");
            }

            List<Animal> animals;
            List<MilkProduction> milkProductions;

            if (imported)
            {
                animals = _dbContext.Animals.ToList();
                milkProductions = _dbContext.MilkProductions.ToList();
            }
            else
            {
                dynamic inMemoryData = putIdsOnInMemoryData();
                animals = inMemoryData.Animals;
                milkProductions = inMemoryData.MilkProductions;
            }

            ReportDDto ret = new ReportDDto();
            ret.AverageProduction = 0;
            if (animals != null && milkProductions != null)
            {
                var productions = from a in animals
                                  from p in milkProductions
                                  where a.AnimalId == p.AnimalId &&
                                        (p.Year * 12 + p.Month) >= (value.StartYear * 12 + value.StartMonth) &&
                                        (p.Year * 12 + p.Month) <= (value.EndYear * 12 + value.EndMonth)
                                  select new ReportDDto() { AverageProduction = p.Quantity };
                var z = productions.ToList();
                ret.AverageProduction = productions.Sum(p => p.AverageProduction) / (value.EndPointInTime - value.StartPointInTime + 1);
            }

            return Ok(ret);
        }

        /// <summary>
        /// Import data on the file 'data.json' to database. Already existing Animals will be skipped.
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK,
            "Animal data was sucessufully imported", Type = typeof(AnimalDto))]
        [HttpPost("import")]
        public ActionResult ImportData()
        {
            var inserts = 0;
            foreach (var animal in _animalData.SelectAllAnimals())
            {
                if (_dbContext.Animals.Any(a => a.Name == animal.Name))
                {
                    _logger.LogWarning($"Ignoring animal {animal.Name}, as it was already imported to the database.");
                    continue;
                }

                _dbContext.Animals.Add(animal);
                ++inserts;
            }

            if (inserts > 0)
            {
                try
                {
                    _dbContext.SaveChanges();
                    AnimalData.DataImported = true;
                }
                catch (Exception e)
                {
                    AnimalData.DataImported = false;
                    _logger.LogError(e.Message + "\n" + e.InnerException);
                }
            }

            return Ok();
        }

        private object putIdsOnInMemoryData()
        {
            List<Animal> animals = new List<Animal>(_animalData.SelectAllAnimals());
            List<FoodUsage> foodUsages = new List<FoodUsage>();
            List<MilkProduction> milkProductions = new List<MilkProduction>();
            int animalId = 0;
            int foodUsageId = 0;
            int milkProductionId = 0;
            foreach (var animal in animals)
            {
                animal.AnimalId = ++animalId;

                if (animal.FoodUsages != null)
                {
                    foreach (var foodUsage in animal.FoodUsages)
                    {
                        foodUsage.AnimalId = animal.AnimalId;
                        foodUsage.FoodUsageId = ++foodUsageId;
                        foodUsages.Add(foodUsage);
                    }
                }

                if (animal.MilkProductions != null)
                {
                    foreach (var milkProduction in animal.MilkProductions)
                    {
                        milkProduction.AnimalId = animal.AnimalId;
                        milkProduction.MilkProductionId = ++milkProductionId;
                        milkProductions.Add(milkProduction);
                    }
                }
            }

            return new { Animals = animals, FoodUsages = foodUsages, MilkProductions = milkProductions };
        }
    }
}
