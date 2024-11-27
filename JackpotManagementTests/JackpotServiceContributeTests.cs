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
        public async Task ContributeToJackpotBalanceAsync_AddsContribution_WhenAmountIsValid()
        {
            decimal contribution = 500;
            var currentJackpotBalance = new JackpotDto { Amount = 1000 };
            var playerBalance = 1000m; // Player has enough balance to contribute

            // Mock the repository methods
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync(currentJackpotBalance);
            _mockJackpotRepository.Setup(repo => repo.UpdateJackpotBalanceAsync(It.IsAny<decimal>())).ReturnsAsync(true);

            // Mock player repository to return a player with sufficient balance
            _mockPlayerRepository.Setup(repo => repo.GetPlayerBalanceAsync("player123")).ReturnsAsync(new PlayerDto { Balance = playerBalance });
            _mockPlayerRepository.Setup(repo => repo.UpdatePlayerBalanceAsync("player123", It.IsAny<decimal>())).ReturnsAsync(true);

            // Act
            var result = await _jackpotService.ContributeToJackpotBalanceAsync(contribution, "player123");

            // Assert
            Assert.True(result); // The contribution should be successful
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once); // Verify that we retrieved the jackpot balance once
            _mockJackpotRepository.Verify(repo => repo.UpdateJackpotBalanceAsync(1500), Times.Once); // 1000 + 500 = 1500
            _mockPlayerRepository.Verify(repo => repo.GetPlayerBalanceAsync("player123"), Times.Once); // Ensure the player's balance was checked
            _mockPlayerRepository.Verify(repo => repo.UpdatePlayerBalanceAsync("player123", 500), Times.Once); // Player's new balance should be 500
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