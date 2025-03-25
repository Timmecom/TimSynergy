using Microsoft.AspNetCore.Mvc;
using TimSynergy.API.Models;
using TimSynergy.API.Services;

namespace TimSynergy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InteractionController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<InteractionController> _logger;

        public InteractionController(ICosmosDbService cosmosDbService, ILogger<InteractionController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(string customerId)
        {
            try
            {
                var interactions = await _cosmosDbService.GetInteractionsAsync(customerId);
                return Ok(interactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting interactions for customer {CustomerId}", customerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var interaction = await _cosmosDbService.GetInteractionAsync(id);
                if (interaction == null)
                {
                    return NotFound();
                }
                return Ok(interaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting interaction {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Interaction interaction)
        {
            try
            {
                // Validate that the customer exists
                var customer = await _cosmosDbService.GetCustomerAsync(interaction.CustomerId);
                if (customer == null)
                {
                    return BadRequest($"Customer with ID {interaction.CustomerId} does not exist");
                }

                await _cosmosDbService.AddInteractionAsync(interaction);
                return CreatedAtAction(nameof(Get), new { id = interaction.Id }, interaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating interaction");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Interaction interaction)
        {
            try
            {
                var existingInteraction = await _cosmosDbService.GetInteractionAsync(id);
                if (existingInteraction == null)
                {
                    return NotFound();
                }

                interaction.Id = id;
                await _cosmosDbService.UpdateInteractionAsync(interaction);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating interaction {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var interaction = await _cosmosDbService.GetInteractionAsync(id);
                if (interaction == null)
                {
                    return NotFound();
                }

                await _cosmosDbService.DeleteInteractionAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting interaction {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
