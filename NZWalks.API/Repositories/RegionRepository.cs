using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories.IRepositories;

namespace NZWalks.API.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalksDBContext nZWalksDBContext;

        public RegionRepository(NZWalksDBContext nZWalksDBContext)
        {
            this.nZWalksDBContext = nZWalksDBContext;
        }
        public async Task<List<Region>> GetRegions()
        {
            return await nZWalksDBContext.Regions.ToListAsync();
        }

        public async Task<Region?> GetRegionByIdAsync(Guid id)
        {
            return await nZWalksDBContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region> AddRegionAsync(Region region)
        {
            await nZWalksDBContext.Regions.AddAsync(region);
            await nZWalksDBContext.SaveChangesAsync(); ;
            return region;
        }

        public async Task<Region?> UpdateRegionAsync(Guid id, Region region)
        {
            var regionDomain = await nZWalksDBContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (regionDomain == null)
                return null;

            regionDomain = region;
            await nZWalksDBContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteRegionAsync(Guid id)
        {
            var regionDomain = await nZWalksDBContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (regionDomain == null)
                return null;

            nZWalksDBContext.Regions.Remove(regionDomain);

            await nZWalksDBContext.SaveChangesAsync();
            return regionDomain;
        }
    }
}
