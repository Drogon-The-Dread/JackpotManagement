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
                var player = await _playerRepository.GetPlayerBalanceAsync(playerId);
                if (player == null)
                {
                    throw new ArgumentException("Player not found.", nameof(playerId));
                }

                if (player.Balance < amount)
                {
                    throw new ArgumentException("Insufficient funds to contribute to the jackpot.", nameof(amount));
                }

                decimal newPlayerBalance = player.Balance - amount;
                var playerBalanceUpdated = await _playerRepository.UpdatePlayerBalanceAsync(playerId, newPlayerBalance);
                if (!playerBalanceUpdated)
                {
                    throw new ApplicationException("Failed to update player balance.");
                }

                var currentJackpot = await _jackpotRepository.GetJackpotBalanceAsync();
                decimal newJackpotBalance = currentJackpot?.Amount + amount ?? amount;
                var jackpotUpdated = await _jackpotRepository.UpdateJackpotBalanceAsync(newJackpotBalance);

                return jackpotUpdated;
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

                var player = await _playerRepository.GetPlayerBalanceAsync(playerId);
                if (player == null)
                {
                    throw new ArgumentException("Player not found.", nameof(playerId));
                }

                decimal newPlayerBalance = player.Balance + currentJackpot.Amount;
                var playerBalanceUpdated = await _playerRepository.UpdatePlayerBalanceAsync(playerId, newPlayerBalance);
                if (!playerBalanceUpdated)
                {
                    throw new ApplicationException("Failed to update player balance.");
                }

                var jackpotUpdated = await _jackpotRepository.UpdateJackpotBalanceAsync(0);
                return jackpotUpdated;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error claiming jackpot", ex);
            }
        }

    }
}
