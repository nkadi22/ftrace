using FarmTraceWebServer.Db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FarmTraceWebServer.Core
{
    /// <summary>
    /// Database context interface
    /// </summary>
    public interface IFarmTraceDatabaseContext : IDisposable
    {
        /// <summary>
        /// Animal table
        /// </summary>
        DbSet<Animal> Animals { get; set; }
        /// <summary>
        /// Food Usage table
        /// </summary>
        DbSet<FoodUsage> FoodUsages { get; set; }
        /// <summary>
        /// Milk Production table
        /// </summary>
        public DbSet<MilkProduction> MilkProductions { get; set; }
        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}
