using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

public class TokenService
{
    public string GenerateTokenUser(UserModel user)
    {
        // Cria instancia do manipulador do JWT
        var handler = new JwtSecurityTokenHandler();
        // Pega a key do env
        var keyEncoded = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("KEY_TOKEN"));

        // Cria as credenciais com a key e o tipo de algoritmo
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyEncoded),
            SecurityAlgorithms.HmacSha256Signature
        );

        // Transforma em claims as informações do usurario
        var claims = new ClaimsIdentity();
        // Adiciona as informações de Nome e Email ao Claim
        claims.AddClaim(new Claim("UserId", user.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Name, user.Username));
        claims.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        // Adiciona cada cargo do usuario
        foreach (var role in user.Roles)
        {
            claims.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        // Cria a descrição do token com os claims do usuario, a data de expiração do token e as credenciais para validação do token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddHours(6),
            SigningCredentials = credentials
        };

        // Gera o token usando a descrição
        var token = handler.CreateToken(tokenDescriptor);

        // Converte o token gerado para string e retorna ele
        return handler.WriteToken(token); ;
    }

    public string GenerateTokenDevice(TokenDeviceDTO device)
    {
        // Cria instancia do manipulador do JWT
        var handler = new JwtSecurityTokenHandler();
        // Pega a key do env
        var keyEncoded = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("KEY_TOKEN"));

        // Cria as credenciais com a key e o tipo de algoritmo
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyEncoded),
            SecurityAlgorithms.HmacSha256Signature
        );

        // Transforma em claims as informações do usurario
        var claims = new ClaimsIdentity();
        // Adiciona as informações de Nome e Email ao Claim
        claims.AddClaim(new Claim("DeviceId", device.Id.ToString()));
        claims.AddClaim(new Claim("UserId", device.UserId.ToString()));
        claims.AddClaim(new Claim("DeviceName", device.DeviceName));
        claims.AddClaim(new Claim(ClaimTypes.Role, "Device"));

        // Cria a descrição do token com os claims do usuario, a data de expiração do token e as credenciais para validação do token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddYears(1),
            SigningCredentials = credentials
        };

        // Gera o token usando a descrição
        var token = handler.CreateToken(tokenDescriptor);

        // Converte o token gerado para string e retorna ele
        return handler.WriteToken(token); ;
    }

    // Método para gerar um Hash de um senha usando BCrypt        
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}