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

        // Relacionamento com o usuário criador (para segurança de posse do link)
        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;
    }

    // Entidade Simples de Usuário para Autenticação
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
