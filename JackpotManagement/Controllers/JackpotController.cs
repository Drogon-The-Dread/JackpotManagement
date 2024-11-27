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

        //[HttpGet("balance")]
        //public async Task<IActionResult> GetJackpotBalance()
        //{
        //    //Write the code here.
        //}

        //[HttpPost("contribute")]
        //public async Task<IActionResult> ContributeToJackpot([FromBody] ContributionRequestDto contributionRequest)
        //{
        //    //Write the code here.
        //}

        //[HttpPost("claim")]
        //public async Task<IActionResult> ClaimJackpot([FromBody] ClaimRequestDto claimRequestDto)
        //{
        //    //Write the code here.
        //}
    }
}
