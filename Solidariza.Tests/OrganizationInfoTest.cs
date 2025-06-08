using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Solidariza.Models.Enum;
using System;
using System.Linq;

namespace Solidariza.Tests
{
    public class OrganizationInfoControllerTests
    {
        private readonly ConnectionDB _dbContext;
        private readonly Mock<ValidateOrganizationService> _mockValidateService;
        private readonly OrganizationInfoController _controller;

        public OrganizationInfoControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _mockValidateService = new Mock<ValidateOrganizationService>();
            _controller = new OrganizationInfoController(_dbContext, _mockValidateService.Object);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
        {
            dbContext.Organization_Info.Add(new OrganizationInfo
            {
                OrganizationInfoId = 1,
                UserId = 1,
                ContactName = "Contato Teste",
                ContactPhone = "47983758492",
                PixKey = "11111111111",
                PixType = PixType.CPF,
                BeneficiaryName = "PIX Teste",
                BeneficiaryCity = "Joinville",
                IsOrganizationApproved = false,
                DisapprovalReason = null,
                User = new User { UserId = 1 } // Adiciona um usuário associado para satisfazer a relação
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetOrganizationInfoById_ExistingId_ReturnsOrganizationInfo()
        {
            var result = await _controller.GetOrganizationInfoById(1);
            var okResult = Assert.IsType<ActionResult<OrganizationInfo>>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(1, okResult.Value.OrganizationInfoId);
            Assert.Equal("Contato Teste", okResult.Value.ContactName);
        }

        [Fact]
        public async Task GetOrganizationInfoById_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetOrganizationInfoById(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrganizationInfoByUserId_ExistingUserId_ReturnsOrganizationInfo()
        {
            // Verificar se o dado está no banco
            var seededOrg = _dbContext.Organization_Info.FirstOrDefault(x => x.UserId == 1);
            Assert.NotNull(seededOrg);
            Assert.Equal(1, seededOrg.UserId);
            Assert.Equal("Contato Teste", seededOrg.ContactName);

            // Depuração: Chamar o serviço diretamente para verificar
            var service = new OrganizationInfoService(_dbContext);
            var serviceResult = await service.GetOrganizationInfoByUserId(1);
            Assert.NotNull(serviceResult);
            Assert.Equal(1, serviceResult.UserId);
            Assert.Equal("Contato Teste", serviceResult.ContactName);
        }

        [Fact]
        public async Task GetOrganizationInfoByUserId_NonExistingUserId_ReturnsNotFound()
        {
            var result = await _controller.GetOrganizationInfoByUserId(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrganizationInfoByUserId_ThrowsException_ReturnsProblem()
        {
            var mockController = new OrganizationInfoController(null!, _mockValidateService.Object); // Força exceção de null
            var result = await mockController.GetOrganizationInfoByUserId(1);
            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.IsType<ProblemDetails>(problemResult.Value);
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
            var createdOrg = Assert.IsType<OrganizationInfo>(okResult.Value);
            Assert.Equal(2, createdOrg.UserId);
            Assert.Equal("New Contact", createdOrg.ContactName);
        }

        [Fact]
        public async Task CreateOrganizationInfo_ThrowsException_ReturnsProblem()
        {
            var mockController = new OrganizationInfoController(null!, _mockValidateService.Object); // Força exceção de null
            var newOrgInfo = new NewOrganizationInfo();

            var result = await mockController.CreateOrganizationInfo(newOrgInfo);
            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
            Assert.IsType<ProblemDetails>(problem.Value);
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

            var updatedOrg = await _dbContext.Organization_Info.FindAsync(1);
            Assert.NotNull(updatedOrg);
            Assert.Equal("Updated Contact", updatedOrg.ContactName);
            Assert.Equal("Curitiba", updatedOrg.BeneficiaryCity);
            Assert.Equal(PixType.CNPJ, updatedOrg.PixType);
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
        public async Task DeleteOrganizationInfo_ExistingId_DeletesAndReturnsNoContent()
        {
            var result = await _controller.DeleteOrganizationInfo(1);
            Assert.IsType<NoContentResult>(result);

            var deletedOrg = await _dbContext.Organization_Info.FindAsync(1);
            Assert.Null(deletedOrg);
        }

        [Fact]
        public async Task DeleteOrganizationInfo_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.DeleteOrganizationInfo(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateOrganization_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.ValidateOrganization(999, "12345678000190");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateOrganization_ThrowsException_ReturnsProblem()
        {
            var controller = new OrganizationInfoController(null!, _mockValidateService.Object); // Força exceção ao acessar o contexto
            var result = await controller.ValidateOrganization(-1, null!);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.IsType<ProblemDetails>(objectResult.Value);
        }
    }
}