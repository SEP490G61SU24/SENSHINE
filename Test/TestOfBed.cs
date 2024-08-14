using API.Controllers;
using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace API.UnitTests.Controllers
{
    [TestFixture]
    public class BedControllerTests
    {
        private BedController _controller;
        private Mock<IBedService> _bedServiceMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _bedServiceMock = new Mock<IBedService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new BedController(_bedServiceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task AddBed_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var bedDTO = new BedDTO { BedNumber = "B1", RoomId = 1 };
            var bed = new Bed { Id = 1, BedNumber = "B1", RoomId = 1 };
            _mapperMock.Setup(m => m.Map<Bed>(bedDTO)).Returns(bed);
            _bedServiceMock.Setup(s => s.AddBed(bed)).ReturnsAsync(bed);
            _mapperMock.Setup(m => m.Map<BedDTO>(bed)).Returns(new BedDTO { Id = 1, BedNumber = "B1", RoomId = 1 });

            // Act
            var result = await _controller.AddBed(bedDTO);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("GetBedById", createdResult.ActionName);
            var returnValue = createdResult.Value as BedDTO;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(1, returnValue.Id);
            Assert.AreEqual("B1", returnValue.BedNumber);
        }

        [Test]
        public async Task UpdateBed_ShouldReturnOkResult_WhenBedExists()
        {
            // Arrange
            var bedDTO = new BedDTO { BedNumber = "B2" };
            var updatedBed = new Bed { Id = 1, BedNumber = "B2", RoomId = 1 };
            //_bedServiceMock.Setup(s => s.UpdateBedAsync(1, "B2")).ReturnsAsync(updatedBed);
            _mapperMock.Setup(m => m.Map<BedDTO>(updatedBed)).Returns(new BedDTO { Id = 1, BedNumber = "B2", RoomId = 1 });

            // Act
            var result = await _controller.UpdateBed(1, bedDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as BedDTO;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(1, returnValue.Id);
            Assert.AreEqual("B2", returnValue.BedNumber);
        }
    }
}
