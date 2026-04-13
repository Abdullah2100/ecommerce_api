namespace api.Presentation.dto.Response
{
    public class StoreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string WallpaperImage  { get; set; }
        public string  SmallImage { get; set; }
        public string UserName { get; set; } = "";
        public bool  IsBlocked { get; set; }
        public decimal? Longitude { get; set; } = null;
        public decimal? Latitude { get; set; } = null;
        public Guid UserId { get; set; }
        public DateTime? UpdatedAtAt { get; set; } = null;
    }


    public class StoreStatusDto
    {
        public  Guid StoreId { get; set; }
        public   bool Status { get; set; }
    }
}