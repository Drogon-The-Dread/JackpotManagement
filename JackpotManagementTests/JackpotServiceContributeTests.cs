using Moq;
using JackpotManagement.Services;
using JackpotManagement.Repositories;
using JackpotManagement.Models;

namespace JackpotManagement.Tests
{
    public class JackpotServiceContributeTests
    {
        private readonly Mock<IJackpotRepository> _mockJackpotRepository;
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly JackpotService _jackpotService;

        public JackpotServiceContributeTests()
        {
            _mockJackpotRepository = new Mock<IJackpotRepository>();
            _mockPlayerRepository = new Mock<IPlayerRepository>();

            _jackpotService = new JackpotService(_mockJackpotRepository.Object, _mockPlayerRepository.Object);
        }

        [Fact]
        public async Task ClaimJackpotAsync_SuccessfullyClaimsJackpot_WhenBalanceIsGreaterThanZero()
        {
            // Arrange
            var currentJackpotBalance = new JackpotDto { Amount = 1000 };
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync(currentJackpotBalance);
            _mockJackpotRepository.Setup(repo => repo.UpdateJackpotBalanceAsync(0)).ReturnsAsync(true);

            // Act
            var result = await _jackpotService.ClaimJackpotAsync("player123");

            // Assert
            Assert.True(result);
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once);
            _mockJackpotRepository.Verify(repo => repo.UpdateJackpotBalanceAsync(0), Times.Once);
        }

        [Fact]
        public async Task ClaimJackpotAsync_ReturnsFalse_WhenJackpotBalanceIsZeroOrNull()
        {
            // Arrange
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync((JackpotDto)null);

            // Act
            var result = await _jackpotService.ClaimJackpotAsync("player123");

            // Assert
            Assert.False(result);
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once);
            _mockJackpotRepository.Verify(repo => repo.UpdateJackpotBalanceAsync(It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task ClaimJackpotAsync_ThrowsArgumentException_WhenPlayerIdIsNullOrEmpty()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _jackpotService.ClaimJackpotAsync(null));
            Assert.Equal("Player ID cannot be null or empty. (Parameter 'playerId')", exception.Message);

            exception = await Assert.ThrowsAsync<ArgumentException>(() => _jackpotService.ClaimJackpotAsync(string.Empty));
            Assert.Equal("Player ID cannot be null or empty. (Parameter 'playerId')", exception.Message);
        }

        [Fact]
        public async Task ClaimJackpotAsync_ThrowsApplicationException_WhenExceptionOccurs()
        {
            // Arrange
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _jackpotService.ClaimJackpotAsync("player123"));
            Assert.Equal("Error claiming jackpot", exception.Message);
        }
    }
}