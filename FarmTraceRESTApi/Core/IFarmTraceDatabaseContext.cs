using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Core
{
    public interface IFarmTraceDatabaseContext : IDisposable
    {
        DbSet<Animal> Animals { get; set; }
        DbSet<FoodUsage> FoodUsages { get; set; }
        public DbSet<MilkProduction> MilkProductions { get; set; }
        int SaveChanges();
    }
}
