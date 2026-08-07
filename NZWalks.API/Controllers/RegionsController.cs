using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDBContext dBContext;

        public RegionsController(NZWalksDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [HttpGet]
        public async Task<ActionResult> GetRegions()
        {
            var regions = await dBContext.Regions.ToListAsync();
            return Ok(regions);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetRegionById(Guid id)
        {
            var region = await dBContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (region == null)
                return NotFound();
            return Ok(region);
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
            await dBContext.Regions.AddAsync(regionDomain);
            await dBContext.SaveChangesAsync();

            var regionDTO = new RegionDTO()
            {
                Id = regionDomain.Id,
                Name = regionDomain.Name,
                Code = regionDomain.Code,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };
            return CreatedAtAction(nameof(GetRegionById), new { id = regionDomain.Id }, regionDTO);  // return 201 status code with the location of the newly created resource

        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionDTO updateRegionDTO)
        {
            var regionDomain = await dBContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            regionDomain.Name = updateRegionDTO.Name;
            regionDomain.Code = updateRegionDTO.Code;
            regionDomain.RegionImageUrl = updateRegionDTO.RegionImageUrl;

            await dBContext.SaveChangesAsync();

            var regionDTO = new RegionDTO()
            {
                Id = regionDomain.Id,
                Name = regionDomain.Name,
                Code = regionDomain.Code,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };
            return Ok(regionDTO);

        }
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            var regionDomain = await dBContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            dBContext.Regions.Remove(regionDomain);
            await dBContext.SaveChangesAsync(); //the await is here because 

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
