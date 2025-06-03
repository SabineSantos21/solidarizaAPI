using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Solidariza.Models.Enum;

namespace Solidariza.Tests
{
    public class OrganizationInfoControllerTests
    {
        private readonly OrganizationInfoController _controller;
        private readonly ConnectionDB _dbContext;

        public OrganizationInfoControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _controller = new OrganizationInfoController(_dbContext);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
        {
            dbContext.Organization_Info.Add(new OrganizationInfo
            {
                UserId = 1,
                ContactName = "Contato Teste",
                ContactPhone = "47983758492",
                PixKey = "11111111111",
                PixType = PixType.CPF,
                BeneficiaryName = "PIX Teste",
                BeneficiaryCity = "Joinville",
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetOrganizationInfoById_ExistingId_ReturnsOrganizationInfo()
        {
            var result = await _controller.GetOrganizationInfoById(1);
            var okResult = Assert.IsType<ActionResult<OrganizationInfo>>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetOrganizationInfoById_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetOrganizationInfoById(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrganizationInfoByUserId_NonExistingUserId_ReturnsOk()
        {
            var result = await _controller.GetOrganizationInfoByUserId(99);
            Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public async Task CreateOrganizationInfo_ValidInfo_ReturnsOk()
        {
            var newOrgInfo = new NewOrganizationInfo
            {
                UserId = 2,
                ContactName = "New Contact",
                ContactPhone = "47999998888",
                PixKey = "22222222222",
                PixType = (int)PixType.CPF,
                BeneficiaryName = "New Beneficiary",
                BeneficiaryCity = "São Paulo",
            };

            var result = await _controller.CreateOrganizationInfo(newOrgInfo);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.IsType<OrganizationInfo>(okResult.Value);
        }

        [Fact]
        public async Task PutOrganizationInfo_ExistingId_UpdatesAndReturnsNoContent()
        {
            var updateOrgInfo = new UpdateOrganizationInfo
            {
                ContactName = "Updated Contact",
                ContactPhone = "47988887777",
                PixKey = "33333333333",
                PixType = (int)PixType.CNPJ,
                BeneficiaryName = "Updated Beneficiary",
                BeneficiaryCity = "Curitiba",
            };

            var result = await _controller.PutOrganizationInfo(1, updateOrgInfo);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrganizationInfo_ExistingId_DeletesAndReturnsNoContent()
        {
            var result = await _controller.DeleteOrganizationInfo(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrganizationInfo_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.DeleteOrganizationInfo(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateOrganizationInfo_ThrowsException_ReturnsProblem()
        {
            var mockController = new OrganizationInfoController(null!); // força exceção de null
            var newOrgInfo = new NewOrganizationInfo();

            var result = await mockController.CreateOrganizationInfo(newOrgInfo);

            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
            Assert.IsType<ProblemDetails>(problem.Value);
        }

        [Fact]
        public async Task PutOrganizationInfo_NonExistingId_ReturnsNotFound()
        {
            var update = new UpdateOrganizationInfo
            {
                ContactName = "Name",
                ContactPhone = "123",
                PixKey = "key",
                PixType = (int)PixType.CPF,
                BeneficiaryName = "Ben",
                BeneficiaryCity = "City"
            };

            var result = await _controller.PutOrganizationInfo(999, update);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateOrganization_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.ValidateOrganization(999, "12345678000190");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetOrganizationInfoByUserId_NonExistingUserId_ReturnsNotFound()
        {
            var result = await _controller.GetOrganizationInfoByUserId(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task ValidateOrganization_ThrowsException_ReturnsProblem()
        {
            // Arrange
            var controller = new OrganizationInfoController(null!); // força exceção ao acessar o contexto

            var result = await controller.ValidateOrganization(-1, null!);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.IsType<ProblemDetails>(objectResult.Value);
        }
    }
}