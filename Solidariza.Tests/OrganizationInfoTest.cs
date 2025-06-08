using Xunit;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models;
using Solidariza.Models.Enum;
using Solidariza.Services;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Solidariza.Tests
{
    public class OrganizationInfoServiceTests
    {
        private readonly ConnectionDB _dbContext;
        private readonly OrganizationInfoService _service;

        public OrganizationInfoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);
            _service = new OrganizationInfoService(_dbContext);
        }

        private static void SeedDatabase(ConnectionDB db)
        {
            // Adiciona usuário e salva primeiro
            db.User.Add(new User
            {
                UserId = 200,
                Name = "Tester"
            });
            db.SaveChanges(); // Necessário para persistir o usuário antes do uso abaixo!

            db.Organization_Info.Add(new OrganizationInfo
            {
                OrganizationInfoId = 1,
                UserId = 200,
                ContactName = "Contato Inicial",
                ContactPhone = "47999999999",
                PixKey = "12345678900",
                PixType = PixType.CPF,
                BeneficiaryName = "Benefic Init",
                BeneficiaryCity = "Joinville",
                IsOrganizationApproved = false,
                DisapprovalReason = null,
                User = db.User.First() // Agora funciona: já existe o usuário!
            });

            db.SaveChanges();
        }

        [Fact]
        public async Task GetOrganizationInfoById_ExistingId_ReturnsObject()
        {
            var result = await _service.GetOrganizationInfoById(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.OrganizationInfoId);
            Assert.Equal("Contato Inicial", result.ContactName);
        }

        [Fact]
        public async Task GetOrganizationInfoById_NonExistingId_ReturnsNull()
        {
            var result = await _service.GetOrganizationInfoById(99);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrganizationInfoByUserId_ExistingUser_ReturnsWithUserIncluded()
        {
            var result = await _service.GetOrganizationInfoByUserId(200);
            Assert.NotNull(result);
            Assert.Equal(200, result.UserId);
            Assert.NotNull(result.User);
        }

        [Fact]
        public async Task GetOrganizationInfoByUserId_NonExistingUser_ReturnsNull()
        {
            var result = await _service.GetOrganizationInfoByUserId(9999);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateOrganizationInfo_ValidInput_PersistsAndReturns()
        {
            var info = new NewOrganizationInfo
            {
                UserId = 111,
                ContactName = "Novo",
                ContactPhone = "2123456789",
                PixKey = "abc123",
                PixType = (int)PixType.CNPJ,
                BeneficiaryName = "ABC",
                BeneficiaryCity = "Floripa",
                PixValue = "123.45m"
            };

            var result = await _service.CreateOrganizationInfo(info);

            Assert.NotNull(result);
            Assert.True(result.OrganizationInfoId > 0);
            Assert.Equal("Novo", result.ContactName);

            var fromDb = await _dbContext.Organization_Info.FindAsync(result.OrganizationInfoId);
            Assert.NotNull(fromDb);
            Assert.Equal("Novo", fromDb.ContactName);
            Assert.Equal("123.45m", fromDb.PixValue);
        }

        [Fact]
        public async Task CreateOrganizationInfoCPNJValid_ValidInput_PersistsAndReturns()
        {
            var info = new NewOrganizationInfoCnpjValid
            {
                UserId = 222,
                IsOrganizationApproved = true,
                DisapprovalReason = "Aprovado"
            };

            var result = await _service.CreateOrganizationInfoCPNJValid(info);

            Assert.NotNull(result);
            Assert.Equal(222, result.UserId);
            Assert.True(result.IsOrganizationApproved);
            Assert.Equal("Aprovado", result.DisapprovalReason);
        }

        [Fact]
        public async Task AtualizarOrganizationInfo_FieldsUpdateCorrectly()
        {
            var original = _dbContext.Organization_Info.First();
            var novo = new OrganizationInfo
            {
                PixKey = "novaPix",
                ContactName = "Alterado",
                ContactPhone = "99998888777",
                PixType = PixType.PHONE,
                BeneficiaryName = "BenefN",
                BeneficiaryCity = "Porto Alegre"
            };

            await _service.AtualizarOrganizationInfo(original, novo);

            Assert.Equal("novaPix", original.PixKey);
            Assert.Equal("Alterado", original.ContactName);
            Assert.Equal(PixType.PHONE, original.PixType);
            Assert.Equal("BenefN", original.BeneficiaryName);
        }

        [Fact]
        public async Task AtualizarOrganizationInfoValidate_UpdatesApprovalFields()
        {
            var org = _dbContext.Organization_Info.First();
            var novo = new OrganizationInfo
            {
                IsOrganizationApproved = true,
                DisapprovalReason = "Ok agora"
            };

            var result = await _service.AtualizarOrganizationInfoValidate(org, novo);

            Assert.True(org.IsOrganizationApproved);
            Assert.Equal("Ok agora", org.DisapprovalReason);
            Assert.Same(org, result);
        }

        [Fact]
        public async Task DeleteOrganizationInfo_RemovesFromDb()
        {
            var entity = new OrganizationInfo
            {
                UserId = 333,
                ContactName = "ParaExcluir",
                ContactPhone = "0000",
                PixKey = "excluirpix",
                PixType = PixType.EMAIL,
                BeneficiaryName = "Del",
                BeneficiaryCity = "DelCity"
            };
            _dbContext.Organization_Info.Add(entity);
            await _dbContext.SaveChangesAsync();

            Assert.True(_dbContext.Organization_Info.Any(x => x.UserId == 333));
            await _service.DeleteOrganizationInfo(entity);
            Assert.False(_dbContext.Organization_Info.Any(x => x.UserId == 333));
        }

        [Fact]
        public async Task DeleteOrganizationInfo_NaoExistente_ThrowsException()
        {
            var notInDb = new OrganizationInfo
            {
                OrganizationInfoId = 999
            };
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _service.DeleteOrganizationInfo(notInDb));
        }
    }
}