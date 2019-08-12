using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace stage_parser
{
    public class OfferContext : DbContext
    {
        public DbSet<Offer> Offers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=" + ConfigurationManager.AppSettings.Get("DatabasePath"));
        }
    }

    public class Offer
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string source { get; set; }
        public string url { get; set; }
        public int? floor_level { get; set; }
        public string floor_is_last { get; set; }
        public string Floor_type { get; set; }
        public int? raw_floor_level { get; set; }
        public string description { get; set; }
        public int? Multilevel_floor { get; set; }
    }
}