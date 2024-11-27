using JackpotManagement.Models;

namespace JackpotManagement.Repositories
{
    public interface IPlayerRepository
    {
        Task<PlayerDto> GetPlayerBalanceAsync(string playerId);
        Task<bool> UpdatePlayerBalanceAsync(string playerId, decimal newBalance);
    }
}
