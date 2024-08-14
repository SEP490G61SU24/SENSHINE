using API.Controllers;
using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SenShineSpa
{
    [TestFixture]
    public class UsersTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IMapper> _mapperMock;
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new UsersController(_userServiceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task AddUser_ValidUser_ReturnsOk()
        {
            // Arrange
            var userDto = new UserDTO { UserName = "testUser", Password = "password123" };
            var user = new UserDTO { Id = 1, UserName = "testUser" };
            _userServiceMock.Setup(service => service.AddUser(It.IsAny<UserDTO>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDto);

            // Act
            var result = await _controller.AddUser(userDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(userDto, okResult.Value);
        }

        [Test]
        public async Task AddUser_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Required");

            // Act
            var result = await _controller.AddUser(new UserDTO());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void AddUser_ExistingUsername_ThrowsArgumentException()
        {
            // Arrange
            var userDto = new UserDTO { UserName = "existingUser" };
            _userServiceMock.Setup(service => service.AddUser(It.IsAny<UserDTO>()))
                            .ThrowsAsync(new ArgumentException("Username already exists."));

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _controller.AddUser(userDto));
        }

        [Test]
        public async Task UpdateUser_ValidUser_ReturnsOk()
        {
            // Arrange
            var userDto = new UserDTO { UserName = "updatedUser" };
            var user = new UserDTO { Id = 1, UserName = "updatedUser" };
            _userServiceMock.Setup(service => service.UpdateUser(1, It.IsAny<UserDTO>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDto);

            // Act
            var result = await _controller.UpdateUser(1, userDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(userDto, okResult.Value);
        }

        [Test]
        public async Task UpdateUser_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Required");

            // Act
            var result = await _controller.UpdateUser(1, new UserDTO());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(service => service.UpdateUser(It.IsAny<int>(), It.IsAny<UserDTO>()))
                            .ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.UpdateUser(1, new UserDTO());

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
