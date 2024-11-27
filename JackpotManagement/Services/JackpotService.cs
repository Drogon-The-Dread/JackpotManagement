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

        public async Task<bool> ContributeToJackpotBalanceAsync(decimal amount, string playerId)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Contribution amount must be greater than zero.", nameof(amount));
            }

            try
            {
                var currentJackpot = await _jackpotRepository.GetJackpotBalanceAsync();

                if (currentJackpot == null)
                {
                    currentJackpot = new JackpotDto { Amount = 0 };
                }

                decimal newJackpotBalance = currentJackpot.Amount + amount;

                var updateSuccess = await _jackpotRepository.UpdateJackpotBalanceAsync(newJackpotBalance);

                return updateSuccess;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error contributing to jackpot balance", ex);
            }
        }


        public async Task<bool> ClaimJackpotAsync(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                throw new ArgumentException("Player ID cannot be null or empty.", nameof(playerId));
            }

            try
            {
                var currentJackpot = await _jackpotRepository.GetJackpotBalanceAsync();

                if (currentJackpot == null || currentJackpot.Amount <= 0)
                {
                    return false; // No jackpot to claim
                }

                var updateSuccess = await _jackpotRepository.UpdateJackpotBalanceAsync(0);

                // Step 4: Log the claim (optional)
                // Log the playerId and jackpot claim. This can be implemented if needed for auditing or records.

                return updateSuccess;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error claiming jackpot", ex);
            }
        }

    }
}
