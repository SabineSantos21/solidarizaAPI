using Xunit;
using Microsoft.EntityFrameworkCore;
using Solidariza.Services;
using Solidariza.Models;
using Solidariza.Models.Enum;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Solidariza.Tests
{
    public class CampaignVolunteerServiceTests
    {
        private readonly CampaignVolunteerService _service;
        private readonly ConnectionDB _dbContext;

        public CampaignVolunteerServiceTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase();

            _service = new CampaignVolunteerService(_dbContext);
        }

        private void SeedDatabase()
        {
            var campaign = new Campaign { CampaignId = 1, UserId = 100 };
            var user = new User { UserId = 1, Name = "User1" };

            _dbContext.Campaign.Add(campaign);
            _dbContext.User.Add(user);

            _dbContext.Campaign_Volunteers.AddRange(
                new CampaignVolunteer
                {
                    CampaignVolunteerId = 1,
                    CampaignId = 1,
                    UserId = 1,
                    IsApproved = CampaignVolunteerStatus.PENDING,
                    Campaign = campaign,
                    User = user
                },
                new CampaignVolunteer
                {
                    CampaignVolunteerId = 2,
                    CampaignId = 1,
                    UserId = 1,
                    IsApproved = CampaignVolunteerStatus.APROVED,
                    Campaign = campaign,
                    User = user
                }
            );

            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetCampaignVolunteerById_ExistingId_ReturnsVolunteer()
        {
            var volunteer = await _service.GetCampaignVolunteerById(1);
            Assert.NotNull(volunteer);
            Assert.Equal(1, volunteer.CampaignVolunteerId);
            Assert.NotNull(volunteer.Campaign);
            Assert.NotNull(volunteer.User);
        }

        [Fact]
        public async Task GetCampaignVolunteerById_NonExistingId_ReturnsNull()
        {
            var volunteer = await _service.GetCampaignVolunteerById(999);
            Assert.Null(volunteer);
        }

        [Fact]
        public async Task GetCampaignVolunteersByCampaignId_ReturnsVolunteers()
        {
            var volunteers = await _service.GetCampaignVolunteersByCampaignId(1);
            Assert.NotEmpty(volunteers);
            Assert.All(volunteers, v => Assert.Equal(1, v.CampaignId));
        }

        [Fact]
        public async Task GetCampaignVolunteersByUserId_ReturnsVolunteers()
        {
            var volunteers = await _service.GetCampaignVolunteersByUserId(1);
            Assert.NotEmpty(volunteers);
            Assert.All(volunteers, v => Assert.Equal(1, v.UserId));
        }

        [Fact]
        public async Task CreateCampaignVolunteer_AddsVolunteer()
        {
            var newVolunteer = new NewCampaignVolunteer { CampaignId = 1, UserId = 1 };
            var created = await _service.CreateCampaignVolunteer(newVolunteer);

            Assert.NotNull(created);
            Assert.Equal(CampaignVolunteerStatus.PENDING, created.IsApproved);

            var found = await _dbContext.Campaign_Volunteers.FindAsync(created.CampaignVolunteerId);
            Assert.NotNull(found);
        }

        [Fact]
        public async Task AtualizarCampaignVolunteer_UpdatesStatus()
        {
            var volunteer = await _dbContext.Campaign_Volunteers.FirstAsync();
            var update = new CampaignVolunteer { IsApproved = CampaignVolunteerStatus.APROVED };

            await _service.AtualizarCampaignVolunteer(volunteer, update);

            var updated = await _dbContext.Campaign_Volunteers.FindAsync(volunteer.CampaignVolunteerId);
            Assert.Equal(CampaignVolunteerStatus.APROVED, updated?.IsApproved);
        }

        [Fact]
        public async Task DeletarCampaignVolunteer_RemovesVolunteer()
        {
            var volunteer = await _dbContext.Campaign_Volunteers.FirstAsync();
            await _service.DeletarCampaignVolunteer(volunteer);

            var deleted = await _dbContext.Campaign_Volunteers.FindAsync(volunteer.CampaignVolunteerId);
            Assert.Null(deleted);
        }
    }
}
