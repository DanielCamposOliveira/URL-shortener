namespace API_Data.src.DTOs
{
    public record CreateUrlRequest(string Url);

    public class ExportPagUrlDTO
    {
        public bool IsActive { get; set; }
        public int ClickCount { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public DateTimeOffset? LastAccessedAt { get; set; }
        public string IdOfuscado { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;      
    }

    public class ExportPagUrlResponse
    {
        public List<ExportPagUrlDTO> Urls { get; set; } = new List<ExportPagUrlDTO>();
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalCount { get; set; }
    }
}
