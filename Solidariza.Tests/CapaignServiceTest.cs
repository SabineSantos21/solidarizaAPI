using Xunit;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models;
using Solidariza.Models.Enum;
using Solidariza.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solidariza.Tests.Services
{
    public class CampaignServiceTests
    {
        private readonly ConnectionDB _dbContext;
        private readonly CampaignService _service;

        public CampaignServiceTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _service = new CampaignService(_dbContext);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
        {
            // Mock Organization, User, Campaign
            dbContext.Organization_Info.Add(new OrganizationInfo
            {
                UserId = 1,
                IsOrganizationApproved = true
            });

            dbContext.User.Add(new User
            {
                UserId = 1,
                Name = "Org Teste"
            });

            dbContext.Campaign.Add(new Campaign
            {
                CampaignId = 1,
                Title = "Campanha 1",
                Description = "Desc 1",
                StartDate = new DateTime(2024, 01, 01),
                EndDate = new DateTime(2024, 12, 31),
                Type = CampaignType.Collection,
                Status = CampaignStatus.Active,
                UserId = 1,
                Address = "Rua 1",
                State = "SP",
                City = "São Paulo"
            });
            dbContext.Campaign.Add(new Campaign
            {
                CampaignId = 2,
                Title = "Campanha 2",
                Description = "Desc 2",
                StartDate = new DateTime(2024, 02, 01),
                EndDate = new DateTime(2024, 11, 30),
                Type = CampaignType.Collection,
                Status = CampaignStatus.Completed,
                UserId = 1,
                Address = "Rua 2",
                State = "RJ",
                City = "Rio"
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetCampaigns_RetornaSoCampanhasAtivasEAprovadas()
        {
            // ACT
            var result = await _service.GetCampaigns();

            // ASSERT
            Assert.Single(result);
            Assert.Equal("Campanha 1", result[0].Title);
        }

        [Fact]
        public async Task GetCampaignById_IdExistente_RetornaCampanha()
        {
            var campaign = await _service.GetCampaignById(1);

            Assert.NotNull(campaign);
            Assert.Equal(1, campaign!.CampaignId);
        }

        [Fact]
        public async Task GetCampaignById_IdNaoExiste_RetornaNull()
        {
            var campaign = await _service.GetCampaignById(999);

            Assert.Null(campaign);
        }

        [Fact]
        public async Task GetCampaignByUserId_RetornaCampanhasUsuario()
        {
            var campaigns = await _service.GetCampaignByUserId(1);

            Assert.Equal(2, campaigns.Count); // Seed tem 2 campanhas para UserId=1
            Assert.All(campaigns, c => Assert.Equal(1, c.UserId));
        }

        [Fact]
        public async Task CreateCampaign_AdicionaCampanhaBanco()
        {
            var newCampaign = new NewCampaign
            {
                Title = "Nova Campanha",
                Description = "Descrição Nova",
                StartDate = new DateTime(2025, 01, 01).ToString("yyyy-MM-dd"),
                EndDate = new DateTime(2025, 12, 31).ToString("yyyy-MM-dd"),
                Status = (int)CampaignStatus.Active,
                UserId = 1,
                Type = (int)CampaignType.Collection,
                Address = "Rua Nova",
                State = "MG",
                City = "Belo Horizonte"
            };

            var created = await _service.CreateCampaign(newCampaign);

            Assert.NotNull(created);
            Assert.Equal("Nova Campanha", created.Title);

            // Verifica persistência no contexto
            Assert.Equal(3, _dbContext.Campaign.Count());
        }

        [Fact]
        public async Task AtualizarCampaign_AtualizaCamposCorretos()
        {
            var existing = _dbContext.Campaign.First();
            var update = new Campaign
            {
                Title = "Atualizado",
                Description = "Desc Atualizado",
                StartDate = existing.StartDate.AddDays(2),
                EndDate = existing.EndDate.AddDays(2),
                Status = CampaignStatus.Completed,
                Type = CampaignType.Collection,
                State = "PR",
                City = "Curitiba",
                Address = "Rua X"
            };

            await _service.AtualizarCampaign(existing, update);

            Assert.Equal("Atualizado", existing.Title);
            Assert.Equal("Curitiba", existing.City);
            Assert.Equal(CampaignStatus.Completed, existing.Status);
        }

        [Fact]
        public async Task DeletarCampaign_RemoveCampanha()
        {
            var campaign = _dbContext.Campaign.First();
            await _service.DeletarCampaign(campaign);

            Assert.Single(_dbContext.Campaign); // Iniciou com 2, removeu 1
        }

        [Fact]
        public async Task DeletarCampaign_CampanhaNaoExistente_NaoExcluiNada()
        {
            var fake = new Campaign { CampaignId = 999 };
            // Não adiciona no contexto, espera erro
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _service.DeletarCampaign(fake));
        }
    }
}