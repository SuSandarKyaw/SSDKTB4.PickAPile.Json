using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSDKTB4.PickAPile.WebApi.Services;

namespace SSDKTB4.PickAPile.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PickAPileController : ControllerBase
	{
        private readonly IPickAPileService _pickAPileService;

        public PickAPileController(IPickAPileService pickAPileService)
        {
            _pickAPileService = pickAPileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPilesAsync()
        {
			var piles = await _pickAPileService.GetAllPilesAsync();
			return Ok(piles);
        }

		[HttpGet("{id}")]
		public async Task<IActionResult> GetPileById(int id)
		{
			var pile = await _pickAPileService.GetPileByIdAsync(id);
			return Ok(pile);
		}

		[HttpGet("search")]
		public async Task<IActionResult> SearchPileAsync(string query)
		{
			var searchPile = await _pickAPileService.SearchPilesAsync(query);
			return Ok(searchPile);
		}
	}
}
