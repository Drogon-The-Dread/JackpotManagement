
namespace GameManagement.Services
{

    public class GameServiceTests
    {
        private readonly IGameService _gameService;

        public GameServiceTests()
        {
            _gameService = new GameService();
        }

        [Fact]
        public void CheckWin_RowWin_ReturnsTrue()
        {
            char[,] grid = {
            { 'X', 'X', 'X' },
            { 'O', ' ', ' ' },
            { 'O', ' ', ' ' }
        };
            Assert.True(_gameService.CheckWin(grid));
        }

        [Fact]
        public void CheckWin_NoWin_ReturnsFalse()
        {
            char[,] grid = {
            { 'X', 'O', 'X' },
            { 'O', 'X', 'O' },
            { 'O', 'X', 'O' }
        };
            Assert.False(_gameService.CheckWin(grid));
        }

        [Fact]
        public void CheckWin_DiagonalWin_ReturnsTrue()
        {
            char[,] grid = {
            { 'X', 'O', 'O' },
            { 'O', 'X', ' ' },
            { ' ', 'O', 'X' }
        };
            Assert.True(_gameService.CheckWin(grid));
        }
    }
}