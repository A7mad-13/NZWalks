using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AutoMapper;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDBContext dBContext;
        private readonly IMapper mapper;

        public IRegionRepository _regionRepository { get; }

        public RegionsController(NZWalksDBContext dBContext, IRegionRepository regionRepository, IMapper mapper )
        {
            this.dBContext = dBContext;
            _regionRepository = regionRepository;
            this.mapper = mapper;
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
            var regionDTO = mapper.Map<RegionDTO> (region);
            return Ok(regionDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegion([FromBody] AddRegionDTO addRegionDTO)
        {

            var regionDomain = mapper.Map<Region>(addRegionDTO);

            var regionmodel = await _regionRepository.AddRegionAsync(regionDomain);

            var regionDTO = mapper.Map<RegionDTO>(regionmodel);
            return CreatedAtAction(nameof(GetRegionById), new { id = regionmodel.Id }, regionDTO);  // return 201 status code with the location of the newly created resource

        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionDTO updateRegionDTO)
        {

            var updateRegion = mapper.Map<Region>(updateRegionDTO);

            updateRegion = await _regionRepository.UpdateRegionAsync(id, updateRegion);

            if (updateRegion == null)
            {
                return NotFound();
            }

            await dBContext.SaveChangesAsync();

            var regionDTO = mapper.Map<RegionDTO>(updateRegion);

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

            var regionDTO = mapper.Map<RegionDTO>(regionDomain);

            return Ok(regionDTO);
        }
    }
}
