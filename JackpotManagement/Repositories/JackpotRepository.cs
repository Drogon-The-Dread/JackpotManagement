using JackpotManagement.Models;

namespace JackpotManagement.Repositories
{
    public class JackpotRepository : IJackpotRepository
    {
        private static readonly JackpotDto _jackpotDto = new JackpotDto { Amount = 1000m };

        public Task<JackpotDto> GetJackpotBalanceAsync()
        {
            return Task.FromResult(_jackpotDto);
        }

        public Task<bool> UpdateJackpotBalanceAsync(decimal newAmount)
        {
            _jackpotDto.Amount = newAmount;
            return Task.FromResult(true);
        }
    }
}
