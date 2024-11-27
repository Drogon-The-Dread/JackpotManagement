using Moq;
using JackpotManagement.Services;
using JackpotManagement.Repositories;
using JackpotManagement.Models;

namespace JackpotManagement.Tests
{
    public class JackpotServiceClaimTests
    {
        private readonly Mock<IJackpotRepository> _mockJackpotRepository;
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly JackpotService _jackpotService;

        public JackpotServiceClaimTests()
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
        public async Task ContributeToJackpotBalanceAsync_ThrowsArgumentException_WhenAmountIsZeroOrNegative()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _jackpotService.ContributeToJackpotBalanceAsync(0, "player123"));
            Assert.Equal("Contribution amount must be greater than zero. (Parameter 'amount')", exception.Message);

            exception = await Assert.ThrowsAsync<ArgumentException>(() => _jackpotService.ContributeToJackpotBalanceAsync(-100, "player123"));
            Assert.Equal("Contribution amount must be greater than zero. (Parameter 'amount')", exception.Message);
        }

        [Fact]
        public async Task ContributeToJackpotBalanceAsync_ReturnsFalse_WhenUpdateFails()
        {
            decimal contribution = 500;
            var currentJackpotBalance = new JackpotDto { Amount = 1000 };
            var playerBalance = 1000m; // Player has enough balance to contribute

            // Mock the repository methods
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync(currentJackpotBalance);
            _mockJackpotRepository.Setup(repo => repo.UpdateJackpotBalanceAsync(It.IsAny<decimal>())).ReturnsAsync(false); // Simulate failure

            // Mock player repository to return a player with sufficient balance
            _mockPlayerRepository.Setup(repo => repo.GetPlayerBalanceAsync("player123")).ReturnsAsync(new PlayerDto { Balance = playerBalance });
            _mockPlayerRepository.Setup(repo => repo.UpdatePlayerBalanceAsync("player123", It.IsAny<decimal>())).ReturnsAsync(true);

            // Act
            var result = await _jackpotService.ContributeToJackpotBalanceAsync(contribution, "player123");

            // Assert
            Assert.False(result); // The result should be false since jackpot update failed
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once); // Verify that we retrieved the jackpot balance once
            _mockJackpotRepository.Verify(repo => repo.UpdateJackpotBalanceAsync(1500), Times.Once); // Attempted jackpot update with 1000 + 500 = 1500
            _mockPlayerRepository.Verify(repo => repo.GetPlayerBalanceAsync("player123"), Times.Once); // Ensure the player's balance was checked
            _mockPlayerRepository.Verify(repo => repo.UpdatePlayerBalanceAsync("player123", 500), Times.Once); // Player's balance was updated to 500
        }

        [Fact]
        public async Task ContributeToJackpotBalanceAsync_ThrowsApplicationException_WhenExceptionOccurs()
        {
            decimal contribution = 500;
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _jackpotService.ContributeToJackpotBalanceAsync(contribution, "player123"));
            Assert.Equal("Error contributing to jackpot balance", exception.Message);
        }
    }
}
