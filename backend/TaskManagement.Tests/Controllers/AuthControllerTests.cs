using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Application.Services;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using Xunit;

namespace TaskManagement.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var dto = new AuthDto { Username = "newuser", Password = "password123" };
        var expectedUser = new User { Id = 1, Username = "newuser" };

        _mockAuthService.Setup(s => s.RegisterAsync(dto))
                        .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        // Using reflection to verify the anonymous object properties you defined
        var value = okResult.Value;
        Assert.NotNull(value);

        var messageProp = value.GetType().GetProperty("message")?.GetValue(value, null);
        var userProp = value.GetType().GetProperty("username")?.GetValue(value, null);

        Assert.Equal("User registered successfully", messageProp);
        Assert.Equal("newuser", userProp);
    }

    [Fact]
    public async Task Register_DuplicateUser_ReturnsBadRequest()
    {
        // Arrange
        var dto = new AuthDto { Username = "exists", Password = "password" };
        _mockAuthService.Setup(s => s.RegisterAsync(dto))
                        .ThrowsAsync(new InvalidOperationException("Username already exists."));

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var dto = new AuthDto { Username = "user", Password = "password" };
        var fakeToken = "valid.jwt.token";

        _mockAuthService.Setup(s => s.LoginAsync(dto))
                        .ReturnsAsync(fakeToken);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value;

        // Match the "Token" property (Capital T) from your controller
        var tokenProp = value.GetType().GetProperty("Token")?.GetValue(value, null);
        Assert.Equal(fakeToken, tokenProp);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new AuthDto { Username = "wrong", Password = "wrong" };
        _mockAuthService.Setup(s => s.LoginAsync(dto))
                        .ReturnsAsync((string?)null);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }
}