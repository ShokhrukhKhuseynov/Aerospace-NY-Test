using MagellanTest.Models;
using MagellanTest.Models.Repositories;
using MagellanTest.Models.Types;
using MagellanTest.Models.Validations;
using Microsoft.AspNetCore.Mvc;

namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository = new ItemRepository();

        [HttpPost("item/create")]
        public async Task<IActionResult> CreateItem([FromBody] Item item)
        {
            return Ok(await _itemRepository.AddAsync(item));
        }

        [HttpPost("item")]
        [ValidateId]
        public async Task<IActionResult> GetItemById([FromBody] GetItemRequest request)
        {
            return Ok(await _itemRepository.GetByIdAsync(request.Id));
        }

        [HttpPost("item/total_cost")]
        public async Task<IActionResult> GetTotalCostByItemName([FromBody] GetTotalCostByItemNameRequest request)
        {
            return Ok(await _itemRepository.GetTotalCostByNameAsync(request.Name));
        }
    }
}