using API.Services;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using AutoMapper;
using API.Dtos;
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

            var cards = _cardService.GetCards().Where(c => c.CardNumber.Trim().ToUpper() == cardDTO.CardNumber.Trim().ToUpper()).FirstOrDefault();

            if (cards != null)
            {
                ModelState.AddModelError("", "Thẻ đã tồn tại");
                return StatusCode(422, ModelState);
            }

            try
            {
                var cardMap = _mapper.Map<Card>(cardDTO);
                var createdCard = await _cardService.CreateCard(cardMap);

                return Ok($"Tạo thẻ {createdCard.CardNumber} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi tạo thẻ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cards = _mapper.Map<List<CardDTO>>(_cardService.GetCards());

            return Ok(cards);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (!_cardService.CardExist(id))
                return NotFound();

            var card = _mapper.Map<CardDTO>(_cardService.GetCard(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(card);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] CardDTO cardDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cardService.CardExist(id))
                return NotFound();

            try
            {
                var existingCard = _cardService.GetCard(id);
                existingCard.CustomerId = cardDTO.CustomerId;
                existingCard.Status = cardDTO.Status;
                var cardUpdate = await _cardService.UpdateCard(id, existingCard);

                if (cardUpdate == null)
                {
                    return NotFound("Không thể cập nhật thẻ");
                }

                return Ok($"Cập nhật thẻ {cardUpdate.CardNumber} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi cập nhật thẻ: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActiveDeactive(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cardService.CardExist(id))
                return NotFound();

            try
            {
                var cardActive = await _cardService.ActiveDeactiveCard(id);

                if (cardActive == null)
                {
                    return NotFound("Không thể chuyển trạng thái thẻ");
                }

                return Ok($"Chuyển trạng thái thẻ {cardActive.CardNumber} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi chuyển trạng thái thẻ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCardComboByCard(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var combo = _mapper.Map<List<CardComboDTO>>(_cardService.GetCardComboByCard(id));

            return Ok(combo);
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

                return Ok($"Thêm combo thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi thêm combo: {ex.Message}");
            }
        }
    }
}
