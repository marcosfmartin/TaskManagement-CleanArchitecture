using Moq;
using Microsoft.Extensions.Configuration;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using BCrypt.Net;

namespace TaskManagement.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _configMock = new Mock<IConfiguration>();

        // Mocking IConfiguration to return a dummy secret key for JWT generation
        _configMock.Setup(c => c["AppSettings:Token"])
                   .Returns("This_Is_A_Super_Secret_Key_That_Is_Exactly_Sixty_Four_Characters_!");

        _authService = new AuthService(_userRepoMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ValidUser_ShouldHashPasswordAndSave()
    {
        // Arrange
        var dto = new AuthDto { Username = "newuser", Password = "SecretPassword123" };

        _userRepoMock.Setup(repo => repo.GetByUsernameAsync(dto.Username))
                     .ReturnsAsync((User?)null); // Username is available

        _userRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                     .ReturnsAsync((User u) => u);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Username, result.Username);
        Assert.NotEqual(dto.Password, result.PasswordHash); // Prove hashing happened
        _userRepoMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateUsername_ShouldThrowException()
    {
        // Arrange
        var dto = new AuthDto { Username = "existinguser", Password = "password" };

        _userRepoMock.Setup(repo => repo.GetByUsernameAsync(dto.Username))
                     .ReturnsAsync(new User { Username = "existinguser" });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnJwtToken()
    {
        // Arrange
        var password = "RealPassword";
        // We must hash it manually in the setup so BCrypt.Verify works
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User { Id = 1, Username = "lucky", PasswordHash = hashed };

        _userRepoMock.Setup(repo => repo.GetByUsernameAsync("lucky"))
                     .ReturnsAsync(user);

        var loginDto = new AuthDto { Username = "lucky", Password = password };

        // Act
        var token = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ShouldReturnNull()
    {
        // Arrange
        var hashed = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User { Username = "tester", PasswordHash = hashed };

        _userRepoMock.Setup(repo => repo.GetByUsernameAsync("tester"))
                     .ReturnsAsync(user);

        var loginDto = new AuthDto { Username = "tester", Password = "WrongPassword" };

        // Act
        var token = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.Null(token);
    }
}