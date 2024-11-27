using JackpotManagement.Models;

namespace JackpotManagement.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private static readonly Dictionary<string, PlayerDto> _players = new Dictionary<string, PlayerDto>
        {
            {"player1", new PlayerDto {PlayerId = "player1", Balance = 500m } },
            {"player2", new PlayerDto {PlayerId = "player2", Balance = 500m } }
        };

        public Task<PlayerDto> GetPlayerBalanceAsync(string playerId) 
        { 
            if(_players.TryGetValue(playerId, out var player))
            {
                return Task.FromResult(player);
            }
            return Task.FromResult<PlayerDto>(null);    
        }

        public Task<bool> UpdatePlayerBalanceAsync(string playerId, decimal newBalance)
        {
            if(_players.ContainsKey(playerId))
            {
                _players[playerId].Balance = newBalance;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
