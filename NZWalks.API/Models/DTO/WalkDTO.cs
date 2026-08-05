namespace NZWalks.API.Models.DTO
{
    public class WalkDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double LengthInKM { get; set; }
        public Guid RegionId { get; set; }
        public Guid DifficultyId { get; set; }
        public string? WalkImageUrl { get; set; }

        //Navigation Properties
        public RegionDTO Region { get; set; }
        public DifficultyDTO Difficulty { get; set; }
    }
}
