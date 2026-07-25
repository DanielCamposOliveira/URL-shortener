using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Models
{
    [Table("urls")]
    public class Url
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Gerado externamente pelo Snowflake (API 1)
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(15)]
        [Column("id_ofuscado")]
        public string IdOfuscado { get; set; } = string.Empty;

        [Required]
        [MaxLength(2048)]
        [Column("original_url")]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required]
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("expires_at")]
        public DateTimeOffset? ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddDays(60); // Expira em 60 dias por padrão

        [Required]
        [Column("click_count")]
        public int ClickCount { get; set; } = 0; // Contador de cliques no link

        [Column("last_accessed_at")]
        public DateTimeOffset? LastAccessedAt { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; } = true; // Indica se o link está ativo por padrão

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        // Propriedade de navegação configurando a chave estrangeira
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
