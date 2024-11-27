using JackpotManagement.Services;
using JackpotManagement.Repositories;
using JackpotManagement.Models;
using Moq;

namespace JackpotManagement.Tests
{
    public class JackpotServiceBalanceTests
    {
        private readonly Mock<IJackpotRepository> _mockJackpotRepository;
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly JackpotService _jackpotService;

        public JackpotServiceBalanceTests()
        {
            _mockJackpotRepository = new Mock<IJackpotRepository>();
            _mockPlayerRepository = new Mock<IPlayerRepository>();

            _jackpotService = new JackpotService(_mockJackpotRepository.Object, _mockPlayerRepository.Object);
        }

        [Fact]
        public async Task GetJackpotBalanceAsync_ReturnsJackpotBalance_WhenBalanceIsNotNull()
        {
            var expectedJackpotBalance = new JackpotDto { Amount = 1000 };
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync(expectedJackpotBalance);

            var result = await _jackpotService.GetJackpotBalanceAsync();

            Assert.NotNull(result);
            Assert.Equal(expectedJackpotBalance.Amount, result.Amount);
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once);
        }

        [Fact]
        public async Task GetJackpotBalanceAsync_ReturnsZero_WhenBalanceIsNull()
        {
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ReturnsAsync((JackpotDto)null);

            var result = await _jackpotService.GetJackpotBalanceAsync();

            Assert.NotNull(result);
            Assert.Equal(0, result.Amount);
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once);
        }

        [Fact]
        public async Task GetJackpotBalanceAsync_ThrowsApplicationException_WhenExceptionOccurs()
        {
            _mockJackpotRepository.Setup(repo => repo.GetJackpotBalanceAsync()).ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _jackpotService.GetJackpotBalanceAsync());
            Assert.Equal("Error retrieving jackpot balance", exception.Message);
            Assert.IsType<ApplicationException>(exception);
            _mockJackpotRepository.Verify(repo => repo.GetJackpotBalanceAsync(), Times.Once);
        }
    }
}
