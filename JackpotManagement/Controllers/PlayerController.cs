using JackpotManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace JackpotManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        // Get player balance
        [HttpGet("{playerId}/balance")]
        public async Task<IActionResult> GetPlayerBalanceAsync(string playerId)
        {
            var player = await _playerRepository.GetPlayerBalanceAsync(playerId);
            if (player == null)
            {
                return NotFound(new { Message = "Player not found" });
            }

            return Ok(new { PlayerId = player.PlayerId, Balance = player.Balance });
        }
    }
}
