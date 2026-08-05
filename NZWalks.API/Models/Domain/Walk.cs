namespace NZWalks.API.Models.Domain
{
    public class Walk
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double LengthInKM { get; set; }
        public Guid RegionId { get; set; }
        public Guid DifficultyId { get; set; }
        public string? WalkImageUrl { get; set; }

        //Navigation Properties
        public Region Region { get; set; }
        public Difficulty Difficulty { get; set; }
    }
}
