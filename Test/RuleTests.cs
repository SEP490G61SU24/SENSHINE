using API.Controllers;
using API.Dtos;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SenShineSpa
{
    [TestFixture]
    public class RuleTests
    {
        private Mock<IRuleService> _mockRuleService;
        private Mock<IMapper> _mockMapper;
        private RuleController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRuleService = new Mock<IRuleService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new RuleController(_mockRuleService.Object, _mockMapper.Object);
        }

        [Test]
        public async Task Create_WithValidRuleDTO_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var ruleDto = new RuleDTO { Title = "New Rule", Path = "/new-rule" };
            var createdRule = new RuleDTO { Id = 1, Title = "New Rule", Path = "/new-rule" };
            _mockRuleService.Setup(s => s.AddRule(ruleDto)).ReturnsAsync(createdRule);

            // Act
            var result = await _controller.Create(ruleDto);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(_controller.GetById), createdAtActionResult.ActionName);
            Assert.AreEqual(1, createdAtActionResult.RouteValues["id"]);
            Assert.AreEqual(createdRule, createdAtActionResult.Value);
        }

        [Test]
        public async Task GetRulesByRoleId_WithValidRoleId_ShouldReturnOkResult_WithListOfRules()
        {
            // Arrange
            int roleId = 1;
            var rules = new List<RuleDTO>
            {
                new RuleDTO { Id = 1, Title = "Rule 1", Path = "/rule1" },
                new RuleDTO { Id = 2, Title = "Rule 2", Path = "/rule2" }
            };
            _mockRuleService.Setup(s => s.GetRulesByRoleId(roleId)).ReturnsAsync(rules);

            // Act
            var result = await _controller.GetRulesByRoleId(roleId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnRules = okResult.Value as IEnumerable<RuleDTO>;
            Assert.AreEqual(2, returnRules.Count());
        }

        [Test]
        public async Task GetRulesByRoleId_WithInvalidRoleId_ShouldReturnNotFoundResult()
        {
            // Arrange
            int roleId = 99; // Invalid RoleId
            _mockRuleService.Setup(s => s.GetRulesByRoleId(roleId)).ReturnsAsync(Enumerable.Empty<RuleDTO>());

            // Act
            var result = await _controller.GetRulesByRoleId(roleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("No rules found for this role.", notFoundResult.Value);
        }
    }
}
