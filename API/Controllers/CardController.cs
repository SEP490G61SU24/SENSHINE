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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the card: {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the cards: {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the card: {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the cards: {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the card: {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while changing card status: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCardComboByCard(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var combo = _mapper.Map<List<CardComboDTO>>(_cardService.GetCardComboByCard(id));
                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the card combo: {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the combo: {ex.Message}");
            }
        }
    }
}
