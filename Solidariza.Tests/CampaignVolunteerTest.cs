using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solidariza.Interfaces.Services;

namespace Solidariza.Tests
{
    public class CampaignVolunteerControllerTests
    {
        private readonly CampaignVolunteerController _controller;
        private readonly ConnectionDB _dbContext;
        public Mock<ICampaignVolunteerService> _campaignVolunteerService { get; set; }


        public CampaignVolunteerControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _campaignVolunteerService = new Mock<ICampaignVolunteerService>();
            _controller = new CampaignVolunteerController(_dbContext, _campaignVolunteerService.Object);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
        {
            if (!dbContext.Campaign_Volunteers.Any())
            {
                dbContext.Campaign_Volunteers.Add(new CampaignVolunteer
                {
                    CampaignId = 1,
                    UserId = 1,
                    IsApproved = CampaignVolunteerStatus.PENDING
                });

                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GetCampaignVolunteerById_NonExistingId_ReturnsNotFound()
        {
            int testId = 99;
            var result = await _controller.GetCampaignVolunteerById(testId);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaignVolunteerByCampaignId_ExistingCampaignId_ReturnsVolunteers()
        {
            int testCampaignId = 1;

            var volunteers = new List<CampaignVolunteer>
            {
                new CampaignVolunteer { CampaignId = testCampaignId, UserId = 1, IsApproved = CampaignVolunteerStatus.PENDING }
            };

            _campaignVolunteerService
                .Setup(s => s.GetCampaignVolunteersByCampaignId(testCampaignId))
                .ReturnsAsync(volunteers);

            var result = await _controller.GetCampaignVolunteerByCampaignId(testCampaignId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedVolunteers = Assert.IsType<List<CampaignVolunteer>>(okResult.Value);
            Assert.NotEmpty(returnedVolunteers);
        }


        [Fact]
        public async Task GetCampaignVolunteerByUserId_ExistingUserId_ReturnsVolunteers()
        {
            int testUserId = 1;

            var volunteers = new List<CampaignVolunteer>
            {
                new CampaignVolunteer { CampaignId = 1, UserId = testUserId, IsApproved = CampaignVolunteerStatus.PENDING }
            };

            _campaignVolunteerService
                .Setup(s => s.GetCampaignVolunteersByUserId(testUserId))
                .ReturnsAsync(volunteers);

            var result = await _controller.GetCampaignVolunteerByUserId(testUserId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedVolunteers = Assert.IsType<List<CampaignVolunteer>>(okResult.Value);
            Assert.NotEmpty(returnedVolunteers);
        }


        [Fact]
        public async Task GetCampaignVolunteerByUserIdAndAproved_ExistingUserId_ReturnsApprovedVolunteers()
        {
            int testUserId = 1;

            var approvedVolunteers = new List<CampaignVolunteer>
            {
                new CampaignVolunteer
                {
                    CampaignId = 1,
                    UserId = testUserId,
                    IsApproved = CampaignVolunteerStatus.APROVED // Ou o valor correto para "aprovado"
                }
            };

            _campaignVolunteerService
                .Setup(s => s.GetCampaignVolunteersByUserIdAndAproved(testUserId))
                .ReturnsAsync(approvedVolunteers);

            var result = await _controller.GetCampaignVolunteerByUserIdAndAproved(testUserId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedVolunteers = Assert.IsType<List<CampaignVolunteer>>(okResult.Value);
            Assert.NotEmpty(returnedVolunteers);
        }


        [Fact]
        public async Task CreateCampaignVolunteer_NewVolunteer_ReturnsCreatedVolunteer()
        {
            var newVolunteer = new NewCampaignVolunteer
            {
                CampaignId = 2,
                UserId = 2
            };

            var createdVolunteer = new CampaignVolunteer
            {
                CampaignId = 2,
                UserId = 2,
                IsApproved = CampaignVolunteerStatus.PENDING
            };

            _campaignVolunteerService
                .Setup(s => s.CreateCampaignVolunteer(It.IsAny<NewCampaignVolunteer>()))
                .ReturnsAsync(createdVolunteer);

            var result = await _controller.CreateCampaignVolunteer(newVolunteer);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVolunteer = Assert.IsType<CampaignVolunteer>(okResult.Value);
            Assert.Equal(2, returnedVolunteer.UserId);
        }


        [Fact]
        public async Task PutCampaignVolunteer_ExistingId_UpdatesVolunteerStatus()
        {
            int testId = 1;
            var updateVolunteer = new UpdateCampaignVolunteer
            {
                IsApproved = (int)CampaignVolunteerStatus.PENDING
            };

            var result = await _controller.PutCampaignVolunteer(testId, updateVolunteer);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCampaignVolunteer_ExistingId_DeletesVolunteer()
        {
            int testId = 1;
            var result = await _controller.DeleteCampaignVolunteer(testId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCampaignVolunteer_NonExistingId_ReturnsNotFound()
        {
            int testId = 99;
            var result = await _controller.DeleteCampaignVolunteer(testId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCampaignVolunteerById_ExistingId_ReturnsVolunteer()
        {
            var existing = _dbContext.Campaign_Volunteers.First();

            // Setup do mock para retornar o volunteer esperado
            _campaignVolunteerService
                .Setup(s => s.GetCampaignVolunteerById(existing.CampaignVolunteerId))
                .ReturnsAsync(existing);

            var result = await _controller.GetCampaignVolunteerById(existing.CampaignVolunteerId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var volunteer = Assert.IsType<CampaignVolunteer>(okResult.Value);
            Assert.NotNull(volunteer);
            Assert.Equal(existing.CampaignVolunteerId, volunteer.CampaignVolunteerId);
        }

        [Fact]
        public async Task CreateCampaignVolunteer_ExistingVolunteer_ReturnsExisting()
        {
            var existing = _dbContext.Campaign_Volunteers.First();

            var newVolunteer = new NewCampaignVolunteer
            {
                CampaignId = existing.CampaignId,
                UserId = existing.UserId
            };

            var result = await _controller.CreateCampaignVolunteer(newVolunteer);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVolunteer = Assert.IsType<CampaignVolunteer>(okResult.Value);

            Assert.Equal(existing.CampaignVolunteerId, returnedVolunteer.CampaignVolunteerId);
        }

        [Fact]
        public async Task PutCampaignVolunteer_NonExistingId_ReturnsNotFound()
        {
            var update = new UpdateCampaignVolunteer
            {
                IsApproved = (int)CampaignVolunteerStatus.APROVED
            };

            var result = await _controller.PutCampaignVolunteer(999, update);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateCampaignVolunteer_WithException_ReturnsProblem()
        {
            var newVolunteer = new NewCampaignVolunteer
            {
                CampaignId = 99,
                UserId = 99
            };

            // Força uma exceção ao chamar CreateCampaignVolunteer
            _campaignVolunteerService
                .Setup(s => s.CreateCampaignVolunteer(It.IsAny<NewCampaignVolunteer>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.CreateCampaignVolunteer(newVolunteer);

            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
        }

    }
}