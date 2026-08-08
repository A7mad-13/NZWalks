using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories.IRepositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDBContext dBContext;

        public IRegionRepository _regionRepository { get; }

        public RegionsController(NZWalksDBContext dBContext, IRegionRepository regionRepository)
        {
            this.dBContext = dBContext;
            _regionRepository = regionRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetRegions()
        {
            var regions = await _regionRepository.GetRegions();
            return Ok(regions);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetRegionById(Guid id)
        {
            var region = await _regionRepository.GetRegionByIdAsync(id);
            if (region == null)
                return NotFound();
            var regionDTO = new RegionDTO()
            {
                Id = id,
                Name = region.Name,
                Code = region.Code,
                RegionImageUrl = region.RegionImageUrl,
            };
            return Ok(regionDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegion([FromBody] AddRegionDTO addRegionDTO)
        {
            var regionDomain = new Region()
            {
                Name = addRegionDTO.Name,
                Code = addRegionDTO.Code,
                RegionImageUrl = addRegionDTO.RegionImageUrl,
            };
            var regionmodel = await _regionRepository.AddRegionAsync(regionDomain);

            var regionDTO = new RegionDTO()
            {
                Id = regionmodel.Id,
                Name = regionmodel.Name,
                Code = regionmodel.Code,
                RegionImageUrl = regionmodel.RegionImageUrl,
            };
            return CreatedAtAction(nameof(GetRegionById), new { id = regionmodel.Id }, regionDTO);  // return 201 status code with the location of the newly created resource

        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionDTO updateRegionDTO)
        {
            var updateRegion = new Region()
            {
                Name = updateRegionDTO.Name,
                Code = updateRegionDTO.Code,
                RegionImageUrl = updateRegionDTO.RegionImageUrl,
            };
             updateRegion = await _regionRepository.UpdateRegionAsync(id, updateRegion);

            if (updateRegion == null)
            {
                return NotFound();
            }

            updateRegion.Name = updateRegionDTO.Name;
            updateRegion.Code = updateRegionDTO.Code;
            updateRegion.RegionImageUrl = updateRegionDTO.RegionImageUrl;

            await dBContext.SaveChangesAsync();

            var regionDTO = new RegionDTO()
            {
                Id = updateRegion.Id,
                Name = updateRegion.Name,
                Code = updateRegion.Code,
                RegionImageUrl = updateRegion.RegionImageUrl,
            };
            return Ok(regionDTO);

        }
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            var regionDomain = await _regionRepository.DeleteRegionAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            var regionDTO = new RegionDTO()
            {
                Id = regionDomain.Id,
                Name = regionDomain.Name,
                Code = regionDomain.Code,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };
            return Ok(regionDTO);
        }
    }
}
