using System;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Dtos.Offers;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Offers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OffersController : ControllerBase
    {
        private readonly IOffers _offerService;


        public OffersController(IOffers offerService)
        {
            _offerService = offerService;
        }
        
        // POST: api/offers
        [HttpPost]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateOfferDto dto)
        {
            var result = await _offerService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            // result.Data contains created Offer Id
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        // GET: api/offers
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        {
            var result = await _offerService.GetAllAsync(onlyActive);
            return Ok(result);
        }

        // GET: api/offers/{id}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _offerService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        // PUT: api/offers/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOfferDto dto)
        {
            var result = await _offerService.UpdateAsync(id, dto);
            if (result.IsSuccess)
                return NoContent();

            // return NotFound when resource missing, otherwise BadRequest
            if (result.ErrorCode == Domain.Enums.ErrorCode.NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        // DELETE: api/offers/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _offerService.DeleteAsync(id);
            if (result.IsSuccess)
                return NoContent();

            if (result.ErrorCode == Domain.Enums.ErrorCode.NotFound)
                return NotFound(result);

            return BadRequest(result);
        }
    }
}
