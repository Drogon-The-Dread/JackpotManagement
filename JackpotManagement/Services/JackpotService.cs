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

        //public async Task<JackpotDto> GetJackpotBalanceAsync()
        //{
        //    //Write the code here.
        //}

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
