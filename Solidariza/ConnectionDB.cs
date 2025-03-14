using Solidariza.Models;
using Microsoft.EntityFrameworkCore;

namespace Solidariza
{
    public class ConnectionDB: DbContext
    {
        public DbSet<User> User { get; set; }

        public DbSet<Campaign> Campaign { get; set; }

        public DbSet<Profile> Profile { get; set; }

        public DbSet<Link> Link { get; set; }

        public DbSet<CampaignVolunteer> Campaign_Volunteers { get; set; }

        public DbSet<OrganizationInfo> Organization_Info { get; set; }

        public ConnectionDB(DbContextOptions<ConnectionDB> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User").HasKey(u => u.UserId);

            modelBuilder.Entity<Campaign>().ToTable("Campaign").HasKey(u => u.UserId);

            modelBuilder.Entity<Profile>().ToTable("Profile").HasKey(u => u.ProfileId);

            modelBuilder.Entity<Link>().ToTable("Link").HasKey(u => u.LinkId);

            modelBuilder.Entity<CampaignVolunteer>().ToTable("Campaign_Volunteers").HasKey(u => u.CampaignVolunteerId);

            modelBuilder.Entity<OrganizationInfo>().ToTable("Organization_Info").HasKey(u => u.OrganizationInfoId);
        }

    }
}
