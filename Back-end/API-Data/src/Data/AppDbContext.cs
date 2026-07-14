using API_Data.src.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Configuração do PostgreSQL para usar o tipo de coluna "bigserial" para a chave primária
        public DbSet<Url> Urls => Set<Url>();
        public DbSet<User> Users => Set<User>();

        // Configuração do modelo e índices
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração do tipo de coluna "bigserial" para a chave primária da entidade Url
            base.OnModelCreating(modelBuilder);

            // Índice Único e otimizado com Includes de performance (PostgreSQL suporta INCLUDE)
            modelBuilder.Entity<Url>()
                .HasIndex(u => u.IdOfuscado)
                .IsUnique()
                .IncludeProperties(u => u.OriginalUrl);

            // Configuração do índice único para o campo Email da entidade User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
