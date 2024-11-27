using JackpotManagement.Models;

namespace JackpotManagement.Repositories
{
    public interface IJackpotRepository
    {
        Task<JackpotDto> GetJackpotBalanceAsync();
        Task<bool> UpdateJackpotBalanceAsync(decimal newAmount);
    }
}
