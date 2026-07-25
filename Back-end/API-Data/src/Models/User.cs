using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Models
{
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

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("IsAdmin")]
        public bool IsAdmin { get; set; } = false;

        [Required]
        [Column("isDarkMode")]
        public bool isDarkMode { get; set; } =false;

        [Required]
        [Column("QtdMaxUrl")]
        public int QtdMaxUrl { get; set; } = 10; // Quantidade Maxima de Url o usuario pode cadastrar

        // Relacionamento com os links criados pelo usuário (1 para N)
        public virtual ICollection<Url> Urls { get; set; } = new List<Url>();
    }
}
