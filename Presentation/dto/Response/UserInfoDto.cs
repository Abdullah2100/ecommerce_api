namespace api.Presentation.dto.Response
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string Phone { get; set; }
        public string Email { get; set; }
        public string StoreName { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public string Thumbnail { get; set; }
        public List<AddressDto>? Address { get; set; }
        public Guid? StoreId { get; set; }
    }
}