namespace API_Data.src.DTOs
{
    public record CreateUrlRequest(string Url);

    public class PageUrlDTO
    {
        public bool IsActive { get; set; }
        public int ClickCount { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public DateTimeOffset? LastAccessedAt { get; set; }
        public string IdOfuscado { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;     
        public string Url {  get; set; } = string.Empty;
    }

    public class ExportPagUrlResponse
    {
        public List<PageUrlDTO> Urls { get; set; } = new List<PageUrlDTO>();
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalCount { get; set; }
    }
}
