using JackpotManagement.Models;
using JackpotManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace JackpotManagement.Controllers
{
    [ApiController]
    [Route("jackpot")]
    public class JackpotController : ControllerBase
    {
        private readonly IJackpotService _jackpotService;

        public JackpotController(IJackpotService jackpotService)
        {
            _jackpotService = jackpotService;
        }

        // GET /jackpot/balance
        [HttpGet("balance")]
        public async Task<IActionResult> GetJackpotBalance()
        {
            try
            {
                var jackpotBalance = await _jackpotService.GetJackpotBalanceAsync();
                return Ok(jackpotBalance); 
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        // POST /jackpot/contribute
        [HttpPost("contribute")]
        public async Task<IActionResult> ContributeToJackpot([FromBody] ContributionRequestDto contributionRequest)
        {
            if (contributionRequest == null || contributionRequest.Amount <= 0)
            {
                return BadRequest("Contribution amount must be greater than zero.");
            }

            try
            {
                var result = await _jackpotService.ContributeToJackpotBalanceAsync(contributionRequest.Amount, contributionRequest.PlayerId);
                if (result)
                {
                    return Ok("Contribution successful.");
                }
                else
                {
                    return BadRequest("Failed to contribute to jackpot.");
                }
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST /jackpot/claim
        [HttpPost("claim")]
        public async Task<IActionResult> ClaimJackpot([FromBody] ClaimRequestDto claimRequestDto)
        {
            if (string.IsNullOrEmpty(claimRequestDto?.PlayerId))
            {
                return BadRequest("Player ID is required.");
            }

            try
            {
                var result = await _jackpotService.ClaimJackpotAsync(claimRequestDto.PlayerId);
                if (result)
                {
                    return Ok("Jackpot claimed successfully.");
                }
                else
                {
                    return NotFound("No jackpot available to claim.");
                }
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
