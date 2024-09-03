using Microsoft.EntityFrameworkCore;
using Root.Models.LogTables;
using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Services.DBContext
{
    public class DBEntities : DbContext
    {
        public DBEntities()
        {

        }

        public DBEntities(DbContextOptions<DBEntities> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    
        public DbSet<InventoryUser> InventoryUsers { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<DeviceModeldetail> DeviceModeldetails { get; set; }
    }
}
