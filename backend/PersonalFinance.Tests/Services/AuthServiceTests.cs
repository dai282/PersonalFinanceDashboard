using FluentAssertions;
using Microsoft.Extensions.Configuration;
using PersonalFinance.API.Services;
using PersonalFinance.Core.Entities;

namespace PersonalFinance.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Mock configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JwtSettings:Secret", "TestSecretKeyThatIsAtLeast32CharactersLong!"},
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"},
                {"JwtSettings:ExpiryInMinutes", "60"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _authService = new AuthService(configuration);
        }

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword()
        {
            //Arrange
            var password = "TestPassword123";

            //Act
            var hashedPassword = _authService.HashPassword(password);

            //Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password);

        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            //Arrange
            var password = "TestPassword123";
            var hashedPassword = _authService.HashPassword(password);

            //Act
            var result = _authService.VerifyPassword(password, hashedPassword);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            //Arrange
            var password = "TestPassword123";
            var wrongPassword = "WrongPassword123";
            var hashedPassword = _authService.HashPassword(password);

            //Act
            var result = _authService.VerifyPassword(wrongPassword, hashedPassword);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidToken()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var token = _authService.GenerateJwtToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3); // JWT has 3 parts
        }

        [Fact]
        public void HashPassword_SamePlainText_ShouldProduceDifferentHashes()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var hash1 = _authService.HashPassword(password);
            var hash2 = _authService.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2); // BCrypt uses salt, so hashes differ
        }

    }
}
