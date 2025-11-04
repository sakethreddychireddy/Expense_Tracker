using Expense_Tracker.Data;
using Expense_Tracker.DTO.AuthDtos;
using Expense_Tracker.Models;
using Expense_Tracker.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Service.Implementations
{
    public class AuthServices : IAuthServices
    {
        private readonly AppDbContext appDbContext;
        private readonly IJwtService jwtServices;
        private readonly ILogger<AuthServices> logger;

        public AuthServices(AppDbContext appDbContext, IJwtService jwtServices, ILogger<AuthServices> logger)
        {
            this.appDbContext = appDbContext;
            this.jwtServices = jwtServices;
            this.logger = logger;
        }
        public async Task<bool> UserExistsAsync(string email)
        {
            return await appDbContext.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<User?> RegisterUserAsync(RegisterUserDto dto)
        {
            try
            {
                var user = new User
                {
                    Email = dto.Email,
                    Role = "User"
                };
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
                appDbContext.Users.Add(user);
                await appDbContext.SaveChangesAsync();

                logger.LogInformation("User {Email} registered successfully.", user.Email);

                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering user.");
                throw;
            }
        }
        public async Task<AuthResponseDto> LoginAsync(LoginUserDto loginUserDto)
        {
            try
            {
                // Step 1: Find user by email
                var user = await appDbContext.Users.SingleOrDefaultAsync(u => u.Email == loginUserDto.Email);
                if (user == null)
                {
                    logger.LogWarning("Login failed: No user found with email {Email}", loginUserDto.Email);
                    return new AuthResponseDto
                    {
                        Token = string.Empty,
                        RefreshToken = string.Empty
                    };
                }

                // Step 2: Verify password
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    logger.LogWarning("Login failed: Incorrect password for email {Email}", loginUserDto.Email);
                    return new AuthResponseDto
                    {
                        Token = string.Empty,
                        RefreshToken = string.Empty
                    };
                }

                // Step 3: Generate JWT Access Token
                var accessToken = jwtServices.GenerateToken(user);

                // Step 4: Generate Secure Refresh Token using JwtService
                var refreshToken = jwtServices.GenerateRefreshToken();

                // Step 5: Save refresh token in DB
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    Expires = DateTime.UtcNow.AddDays(7),
                    UserId = user.Id,
                    IsRevoked = false
                };

                appDbContext.RefreshTokens.Add(refreshTokenEntity);
                await appDbContext.SaveChangesAsync();

                // Step 6: Return tokens to frontend
                return new AuthResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login.");
                throw new Exception($"Error during login: {ex.Message}", ex);
            }
        }
        public async Task<bool> LogoutAsync(int userId, string refreshToken)
        {
            try
            {
                // Step 1: Find the refresh token that matches the user and is still active
                var token = await appDbContext.RefreshTokens
                    .FirstOrDefaultAsync(t =>
                        t.UserId == userId &&
                        t.Token == refreshToken &&
                        !t.IsRevoked &&
                        t.Expires > DateTime.UtcNow);

                if (token == null)
                {
                    logger.LogWarning("Logout failed: No valid refresh token found for User {UserId}", userId);
                    return false;
                }

                // Step 2: Revoke the token
                token.IsRevoked = true;
                //token.RevokedAt = DateTime.UtcNow;

                // Step 3: Save changes to database
                appDbContext.RefreshTokens.Update(token);
                await appDbContext.SaveChangesAsync();

                logger.LogInformation("User {UserId} logged out successfully. Refresh token revoked.", userId);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during logout for User {UserId}.", userId);
                return false;
            }
        }
    }
}
