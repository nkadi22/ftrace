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
    public interface IAnimalData
    {
        void Init();
        List<Animal> SelectAllAnimals();
    }
    public class AnimalData: IAnimalData
    {
        public static bool DataImported = false;
        public List<Animal> Animals { get; } = new List<Animal>();
        public void Init()
        {
            //dynamic array = JsonConvert.Dd
            var path = Directory.GetCurrentDirectory() + "\\Data\\data.json";
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
                        //Could not identify the genre, ask the team
                        continue;
                    }

                    // Food quantity validation
                    foreach (var foodUsage in animal.FoodUsage)
                    {
                        int quantity = 0;
                        /*validation*/
                        if (myAnimal.Type == AnimalType.Cow)
                        {
                            //including the zero and the 30?
                            if (foodUsage.Quantity < 0 || foodUsage.Quantity > 30)
                            {
                                //Logs
                                continue;
                            }

                            quantity = foodUsage.Quantity;
                        }
                        else if (myAnimal.Type == AnimalType.Goat)
                        {
                            //including the zero and the 30?
                            if (foodUsage.Quantity < 0 || foodUsage.Quantity > 3)
                            {
                                //Logs
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
                        // Food quantity validation
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

        public List<Animal> SelectAllAnimals()
        {
            return Animals;
        }
    }

    public class FarmTraceDatabaseContext : DbContext, IFarmTraceDatabaseContext
    {
        public FarmTraceDatabaseContext() : base()
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<FarmTraceDatabaseContext>());
            Database.SetInitializer<FarmTraceDatabaseContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FoodUsage>().HasKey(c => new { c.AnimalId, c.Year, c.Month });
            modelBuilder.Entity<MilkProduction>().HasKey(c => new { c.AnimalId, c.Year, c.Month });
        }

        public DbSet<Animal> Animals
        {
            get;
            set;
        }

        public DbSet<FoodUsage> FoodUsages
        {
            get;
            set;
        }

        public DbSet<MilkProduction> MilkProductions
        {
            get;
            set;
        }
    }
}
