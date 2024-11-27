using JackpotManagement.Models;

namespace JackpotManagement.Services
{
    public interface IJackpotService
    {
        Task<JackpotDto> GetJackpotBalanceAsync();
        Task<bool> ContributeToJackpotBalanceAsync(decimal amount, string playerId);
        Task<bool> ClaimJackpotAsync(string playerId);
    }
}
