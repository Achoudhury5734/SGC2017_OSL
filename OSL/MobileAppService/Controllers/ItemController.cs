using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSL.MobileAppService;
using OSL.Models;

namespace OSL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ItemController : Controller
    {

        private readonly IItemRepository ItemRepository;

        public ItemController(IItemRepository itemRepository)
        {
            ItemRepository = itemRepository;
        }

        [HttpGet]
        public IActionResult List()
        {
            var scopes = HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope")?.Value;

            if (!string.IsNullOrEmpty(Startup.ScopeRead) && scopes != null && scopes.Split(' ').Any(s => s.Equals(Startup.ScopeRead)))
            {
                return Ok(ItemRepository.GetAll());
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("{Id}")]
        public Item GetItem(string id)
        {
            Item item = ItemRepository.Get(id);
            return item;
        }

        [HttpPost]
        public IActionResult Create([FromBody]Item item)
        {
            try
            {
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid State");
                }

                ItemRepository.Add(item);

            }
            catch (Exception)
            {
                return BadRequest("Error while creating");
            }
            return Ok(item);
        }

        [HttpPut]
        public IActionResult Edit([FromBody] Item item)
        {
            try
            {
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid State");
                }
                ItemRepository.Update(item);
            }
            catch (Exception)
            {
                return BadRequest("Error while creating");
            }
            return NoContent();
        }

        [HttpDelete("{Id}")]
        public void Delete(string id)
        {
            ItemRepository.Remove(id);
        }
    }
}
