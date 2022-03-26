using FarmTraceWebServer.Db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Core
{
    /// IAnimalData interface
    public interface IAnimalData
    {
        /// <summary>
        /// Imports valid animal data to memory. 
        /// </summary>
        /// <returns></returns>
        void Init();
        /// <summary>
        /// Retrieves the animal data stored in memory. 
        /// </summary>
        /// <returns></returns>
        List<Animal> SelectAllAnimals();
    }

    /// AnimalData class
    public class AnimalData: IAnimalData
    {
        /// Whether the data has been imported
        public static bool DataImported = false;
        /// List of animals
        public List<Animal> Animals { get; } = new List<Animal>();

        /// <summary>
        /// Imports valid animal data to memory. 
        /// </summary>
        /// <returns></returns>
        public void Init()
        {
            var path = Directory.GetCurrentDirectory() + "\\Data\\data.json";
            if (!File.Exists(path))
            {
                return;
            }

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);
                foreach (var animal in array.animals)
                {
                    AnimalType aType;
                    AnimalGenre aGender;

                    Animal myAnimal = new Animal();
                    myAnimal.Name = animal.Name;
                    if (Enum.TryParse<AnimalType>((string)animal.Type, true, out aType))
                    {
                        myAnimal.Type = aType;
                    }
                    else
                    {
                        //Animal other than Cow or Goat, ignore
                        continue;
                    }

                    if (Enum.TryParse<AnimalGenre>((string)animal.Genre, true, out aGender))
                    {
                        myAnimal.Gender = aGender;
                    }
                    else
                    {
                        //Could not identify the genre, ignore animal
                        continue;
                    }

                    // Food quantity validation
                    foreach (var foodUsage in animal.FoodUsage)
                    {
                        int quantity = 0;
                        if (myAnimal.Type == AnimalType.Cow)
                        {
                            //including the zero and the 30?
                            if (foodUsage.Quantity < 0 || foodUsage.Quantity > 30)
                            {
                                continue;
                            }

                            quantity = foodUsage.Quantity;
                        }
                        else if (myAnimal.Type == AnimalType.Goat)
                        {
                            if (foodUsage.Quantity < 0 || foodUsage.Quantity > 3)
                            {
                                continue;
                            }

                            quantity = foodUsage.Quantity;
                        }
                        else
                        {
                            continue;
                        }

                        FoodUsage usage = new FoodUsage();
                        usage.Year = foodUsage.Year;
                        usage.Month = foodUsage.Month;
                        usage.Quantity = quantity;
                        myAnimal.FoodUsages = (myAnimal.FoodUsages == null)? new List<FoodUsage>() : myAnimal.FoodUsages;
                        myAnimal.FoodUsages.Add(usage);
                    }

                    

                    if (myAnimal.Gender == AnimalGenre.Female)
                    {
                        // Milk production validation
                        foreach (var milkProduction in animal.MilkProduction)
                        {
                            int quantity = 0;
                            if (myAnimal.Type == AnimalType.Cow && milkProduction.Quantity >= 0 || milkProduction.Quantity <= 35)
                            {
                                quantity = milkProduction.Quantity;
                            }
                            else if (myAnimal.Type == AnimalType.Goat && milkProduction.Quantity >= 0 || milkProduction.Quantity <= 8)
                            {
                                quantity = milkProduction.Quantity;
                            }
                            else
                            {
                                continue;
                            }

                            MilkProduction production = new MilkProduction();
                            production.Year = milkProduction.Year;
                            production.Month = milkProduction.Month;
                            production.Quantity = quantity;
                            myAnimal.MilkProductions = (myAnimal.MilkProductions == null) ? new List<MilkProduction>() : myAnimal.MilkProductions;
                            myAnimal.MilkProductions.Add(production);
                        }
                    }

                    Animals.Add(myAnimal);
                }
            }
        }

        /// <summary>
        /// Retrieves the animal data stored in memory. 
        /// </summary>
        /// <returns></returns>
        public List<Animal> SelectAllAnimals()
        {
            return Animals;
        }
    }

    /// <summary>
    /// Database context
    /// </summary>
    public class FarmTraceDatabaseContext : DbContext, IFarmTraceDatabaseContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FarmTraceDatabaseContext() : base()
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<FarmTraceDatabaseContext>());
            Database.SetInitializer<FarmTraceDatabaseContext>(null);
        }

        /// <summary>
        /// Composite primary keys
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FoodUsage>().HasKey(c => new { c.AnimalId, c.Year, c.Month });
            modelBuilder.Entity<MilkProduction>().HasKey(c => new { c.AnimalId, c.Year, c.Month });
        }

        /// <summary>
        /// Database Animal table
        /// </summary>
        public DbSet<Animal> Animals
        {
            get;
            set;
        }

        /// <summary>
        /// Food Usage table
        /// </summary>
        public DbSet<FoodUsage> FoodUsages
        {
            get;
            set;
        }

        /// <summary>
        /// Milk Production table
        /// </summary>
        public DbSet<MilkProduction> MilkProductions
        {
            get;
            set;
        }
    }
}
