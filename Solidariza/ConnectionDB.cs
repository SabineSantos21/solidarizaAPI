using Solidariza.Models;
using Microsoft.EntityFrameworkCore;

namespace Solidariza
{
    public class ConnectionDB: DbContext
    {
        public DbSet<Usuario> TbUsuario { get; set; }

        public ConnectionDB(DbContextOptions<ConnectionDB> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Tb_Usuario").HasKey(u => u.Id);
        }

    }
}
