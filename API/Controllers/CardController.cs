using API.Services;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using AutoMapper;
using API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Services.Impl;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CardController : Controller
    {
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;

        public CardController(ICardService cardService, IMapper mapper)
        {
            _cardService = cardService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CardDTO cardDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var card = _cardService.GetAllCards().Where(c => c.CardNumber.Trim().ToUpper() == cardDTO.CardNumber.Trim().ToUpper()).FirstOrDefault();

            if (card != null)
            {
                ModelState.AddModelError("", "The card already exists.");
                return StatusCode(422, ModelState);
            }

            try
            {
                var cardMap = _mapper.Map<Card>(cardDTO);
                var createdCard = await _cardService.CreateCard(cardMap);

                return Ok($"Card '{createdCard.CardNumber}' created successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] string? spaId = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var cards = await _cardService.GetCards(pageIndex, pageSize, searchTerm, spaId);
                return Ok(cards);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (!_cardService.CardExist(id))
                    return NotFound("Card not found.");

                var card = _mapper.Map<CardDTO>(_cardService.GetCard(id));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(card);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByNumNamePhone(string input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!_cardService.CardExistByNumNamePhone(input))
                    return NotFound("Card not found.");

                var cards = _mapper.Map<List<CardDTO>>(_cardService.GetCardByNumNamePhone(input));

                return Ok(cards);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] CardDTO cardDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!_cardService.CardExist(id))
                    return NotFound("Card not found.");

                var existingCard = _cardService.GetCard(id);
                existingCard.CustomerId = cardDTO.CustomerId;
                existingCard.Status = cardDTO.Status;
                var cardUpdate = await _cardService.UpdateCard(id, existingCard);

                if (cardUpdate == null)
                {
                    return NotFound("Failed to update card.");
                }

                return Ok($"Card '{cardUpdate.CardNumber}' updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActiveDeactive(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!_cardService.CardExist(id))
                    return NotFound("Card not found.");

                var cardActive = await _cardService.ActiveDeactiveCard(id);

                if (cardActive == null)
                {
                    return NotFound("Failed to change card status.");
                }

                return Ok($"Card '{cardActive.CardNumber}' status changed successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UseCard(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var cardUsed = await _cardService.UseCardCombo(id);

                if (cardUsed == null)
                {
                    return NotFound("Failed to change card status.");
                }

                return Ok($"Successfully use card.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCardComboByCard(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var combos = _mapper.Map<List<CardComboDTO>>(_cardService.GetCardComboByCard(id));
                return Ok(combos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCombo([FromBody] CardComboDTO cardComboDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var cardComboMap = _mapper.Map<CardCombo>(cardComboDTO);
                var createdCardCombo = await _cardService.CreateCardCombo(cardComboMap);

                return Ok("Combo added successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCardInvoiceByCard(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var invoices = _mapper.Map<List<CardInvoiceDTO>>(_cardService.GetCardInvoiceByCard(id));
                return Ok(invoices);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<InvoiceDTO>> GetInvoice(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var invoice = await _cardService.GetInvoiceById(id);

                if (invoice == null)
                {
                    return NotFound();
                }

                return Ok(invoice);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddInvoice([FromBody] CardInvoiceDTO cardInvoiceDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var cardInvoiceMap = _mapper.Map<CardInvoice>(cardInvoiceDTO);
                var createdCardInvoice = await _cardService.CreateCardInvoice(cardInvoiceMap);

                return Ok("Invoice added successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }
    }
}
