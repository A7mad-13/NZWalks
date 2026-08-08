using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories.IRepositories
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetRegions();
        Task<Region?> GetRegionByIdAsync(Guid id);
        Task<Region> AddRegionAsync(Region region);
        Task<Region?> UpdateRegionAsync(Guid id, Region region);
        Task<Region?> DeleteRegionAsync(Guid id);
    }
}
