using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solidariza.Models.Enum;

namespace Solidariza.Tests
{
    public class LinkControllerTests
    {
        private readonly LinkController _controller;
        private readonly ConnectionDB _dbContext;

        public LinkControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _controller = new LinkController(_dbContext);
        }

        private void SeedDatabase(ConnectionDB dbContext)
        {
            dbContext.Link.Add(new Link
            {
                Url = "www.google.com.br",
                Type = LinkType.OTHER,
                ProfileId = 1,
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetLinksByProfileId_ExistingProfileId_ReturnsLinks()
        {
            int profileId = 1;
            var result = await _controller.GetLinksByProfileId(profileId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetLinksByProfileId_NonExistingProfileId_ReturnsEmptyList()
        {
            int profileId = 99;
            var result = await _controller.GetLinksByProfileId(profileId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var links = Assert.IsType<List<Link>>(okResult.Value);
            Assert.Empty(links);
        }

        [Fact]
        public async Task CreateLink_ValidLink_ReturnsOk()
        {
            var newLink = new NewLink
            {
                Url = "www.example.com",
                Type = LinkType.OTHER,
                ProfileId = 1
            };

            var result = await _controller.CreateLink(newLink);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdLink = Assert.IsType<Link>(okResult.Value);
            Assert.Equal("www.example.com", createdLink.Url);
        }

        [Fact]
        public async Task DeleteLink_ExistingId_DeletesLink()
        {
            var existingLink = new Link
            {
                Url = "www.delete.com",
                Type = LinkType.OTHER,
                ProfileId = 1,
            };

            _dbContext.Link.Add(existingLink);
            _dbContext.SaveChanges();

            var result = await _controller.DeleteLink(existingLink.LinkId);
            Assert.IsType<NoContentResult>(result);

            var deletedLink = await _dbContext.Link.FindAsync(existingLink.LinkId);
            Assert.Null(deletedLink);
        }

        [Fact]
        public async Task DeleteLink_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.DeleteLink(99);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}