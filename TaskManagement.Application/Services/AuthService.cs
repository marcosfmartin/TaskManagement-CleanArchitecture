using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using TaskManagement.Application.Services;
using System.Security.Claims;

namespace TaskManagement.Application.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(AuthDto dto);
    Task<string?> LoginAsync(AuthDto dto);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public Task<User> RegisterAsync(AuthDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<string?> LoginAsync(AuthDto dto)
    {
        throw new NotImplementedException();
    }

    private string CreateToken(User user)
    {
        throw new NotImplementedException();
    }
}