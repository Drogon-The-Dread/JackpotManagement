using JackpotManagement.Models;
using JackpotManagement.Repositories;

namespace JackpotManagement.Services
{
    public class JackpotService: IJackpotService
    {
        private readonly IJackpotRepository _jackpotRepository;
        private readonly IPlayerRepository _playerRepository;

        public JackpotService(IJackpotRepository jackpotRepository, IPlayerRepository playerRepository)
        {
            _jackpotRepository = jackpotRepository;
            _playerRepository = playerRepository;
        }

        public async Task<JackpotDto> GetJackpotBalanceAsync()
        {
            try
            {
                var jackpotBalance = await _jackpotRepository.GetJackpotBalanceAsync();

                if (jackpotBalance == null)
                {
                    jackpotBalance = new JackpotDto { Amount = 0 };
                }

                return jackpotBalance;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving jackpot balance", ex);
            }
        }

        //public async Task<bool> ContributeToJackpotBalanceAsync(decimal amount, string playerId)
        //{
        //    // Write the code here. 
        //}

        //public async Task<bool> ClaimJackpotAsync(string playerId)
        //{
        //    //Write the code here.
        //}
    }
}
