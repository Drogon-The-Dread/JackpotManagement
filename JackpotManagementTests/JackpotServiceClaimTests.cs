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
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync(currentJackpotBalance);
            _mockJackpotRepository.Setup(repo => repo.UpdateJackpotBalanceAsync(It.IsAny<decimal>())).ReturnsAsync(true);

            var result = await _jackpotService.ContributeToJackpotBalanceAsync(contribution, "player123");

            Assert.True(result);
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once);
            _mockJackpotRepository.Verify(repo => repo.UpdateJackpotBalanceAsync(1500), Times.Once); // 1000 + 500 = 1500
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
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync(currentJackpotBalance);
            _mockJackpotRepository.Setup(repo => repo.UpdateJackpotBalanceAsync(It.IsAny<decimal>())).ReturnsAsync(false);

            var result = await _jackpotService.ContributeToJackpotBalanceAsync(contribution, "player123");

            Assert.False(result);
            _mockJackpotRepository.Verify(repo => repo.UpdateJackpotBalanceAsync(1500), Times.Once); // 1000 + 500 = 1500
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
