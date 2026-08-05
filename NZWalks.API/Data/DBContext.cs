using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.Domain;
namespace NZWalks.API.Data
{
    public class NZWalksDBContext: DbContext
    {
        public NZWalksDBContext(DbContextOptions dPContextOptions) : base(dPContextOptions)
        {

        }

        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }
        public DbSet<Difficulty> Difficulties{ get; set; }
    }
}
