using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

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

    public async Task<User> RegisterAsync(AuthDto dto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username is already taken.");
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BC.HashPassword(dto.Password)
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<string?> LoginAsync(AuthDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);

        if (user == null || !BC.Verify(dto.Password, user.PasswordHash))
            return null;

        return CreateToken(user);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var secret = _configuration["AppSettings:Token"]
                     ?? throw new InvalidOperationException("JWT Token key is missing in appsettings.json");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}