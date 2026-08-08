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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ids are hardcoded rather than generated, so re-running a migration
            // does not produce a new set of rows every time.

            // Seed Difficulties
            var difficulties = new List<Difficulty>()
            {
                new Difficulty()
                {
                    Id = Guid.Parse("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"),
                    Name = "Easy"
                },
                new Difficulty()
                {
                    Id = Guid.Parse("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"),
                    Name = "Medium"
                },
                new Difficulty()
                {
                    Id = Guid.Parse("f808ddcd-b5e5-4d80-b732-1ca523e48434"),
                    Name = "Hard"
                }
            };

            modelBuilder.Entity<Difficulty>().HasData(difficulties);

            // Seed Regions
            var regions = new List<Region>()
            {
                new Region()
                {
                    Id = Guid.Parse("f7248fc3-2585-4efb-8d1d-1c555f4087f6"),
                    Name = "Auckland",
                    Code = "AKL",
                    RegionImageUrl = "https://images.pexels.com/photos/5169056/pexels-photo-5169056.jpeg"
                },
                new Region()
                {
                    Id = Guid.Parse("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"),
                    Name = "Northland",
                    Code = "NTL",
                    RegionImageUrl = null
                },
                new Region()
                {
                    Id = Guid.Parse("14ceba71-4b51-4777-9b17-46602cf66153"),
                    Name = "Bay Of Plenty",
                    Code = "BOP",
                    RegionImageUrl = null
                }
            };

            modelBuilder.Entity<Region>().HasData(regions);

            // Seed Walks - RegionId and DifficultyId must match the Ids seeded above
            var walks = new List<Walk>()
            {
                new Walk()
                {
                    Id = Guid.Parse("7e0f4b3a-1c9d-4a2e-9f52-3b8c1d0e6a74"),
                    Name = "Rangitoto Island Summit Track",
                    LengthInKM = 5.5,
                    WalkImageUrl = "https://images.pexels.com/photos/1687845/pexels-photo-1687845.jpeg",
                    RegionId = Guid.Parse("f7248fc3-2585-4efb-8d1d-1c555f4087f6"),
                    DifficultyId = Guid.Parse("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c")
                },
                new Walk()
                {
                    Id = Guid.Parse("2d5a9c81-6b34-4e7f-a1c8-9d02f4b5e3a6"),
                    Name = "Cape Reinga Coastal Walk",
                    LengthInKM = 12.0,
                    WalkImageUrl = null,
                    RegionId = Guid.Parse("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"),
                    DifficultyId = Guid.Parse("f808ddcd-b5e5-4d80-b732-1ca523e48434")
                },
                new Walk()
                {
                    Id = Guid.Parse("b3c17e59-8a4d-42f6-b0e1-5c7d9a2f8b40"),
                    Name = "Mount Maunganui Base Track",
                    LengthInKM = 3.4,
                    WalkImageUrl = null,
                    RegionId = Guid.Parse("14ceba71-4b51-4777-9b17-46602cf66153"),
                    DifficultyId = Guid.Parse("54466f17-02af-48e7-8ed3-5a4a8bfacf6f")
                }
            };

            modelBuilder.Entity<Walk>().HasData(walks);
        }
    }
}
