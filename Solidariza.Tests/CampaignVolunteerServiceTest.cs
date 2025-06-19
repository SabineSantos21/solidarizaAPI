using Xunit;
using Microsoft.EntityFrameworkCore;
using Solidariza.Services;
using Solidariza.Models;
using Solidariza.Models.Enum;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Solidariza.Tests.Services
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
            var org = new User { UserId = 100, Name = "Org" };
            var user = new User { UserId = 1, Name = "User1" };
            var campaign = new Campaign { CampaignId = 1, UserId = 100, User = org };

            _dbContext.User.AddRange(org, user);
            _dbContext.Campaign.Add(campaign);

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

        [Fact]
        public async Task GetCampaignVolunteersByUserIdAndAproved_ReturnsApprovedOnly()
        {
            var volunteers = await _service.GetCampaignVolunteersByUserIdAndAproved(1);
            Assert.Single(volunteers);
            Assert.All(volunteers, v => Assert.Equal(CampaignVolunteerStatus.APROVED, v.IsApproved));
        }

        [Fact]
        public async Task GetCampaignVolunteersByUserIdAndAproved_ReturnsEmpty_WhenNoApproved()
        {
            var volunteers = await _service.GetCampaignVolunteersByUserIdAndAproved(999);
            Assert.Empty(volunteers);
        }

        [Fact]
        public async Task AtualizarCampaignVolunteer_NonExistingVolunteer_ThrowsException()
        {
            var fake = new CampaignVolunteer
            {
                CampaignVolunteerId = 999,
                IsApproved = CampaignVolunteerStatus.PENDING
            };
            var update = new CampaignVolunteer { IsApproved = CampaignVolunteerStatus.APROVED };
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _service.AtualizarCampaignVolunteer(fake, update));
        }

        [Fact]
        public async Task DeletarCampaignVolunteer_NotExistingVolunteer_ThrowsException()
        {
            var fake = new CampaignVolunteer { CampaignVolunteerId = 999 };
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _service.DeletarCampaignVolunteer(fake));
        }
    }
}
