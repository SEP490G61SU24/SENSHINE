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
        public IActionResult GetById(int id)
        {
            if (!_cardService.CardExist(id))
                return NotFound();

            var card = _mapper.Map<CardDTO>(_cardService.GetCard(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(card);
        }

        [HttpGet]
        public async Task<IActionResult> GetByNumNamePhone(string input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cardService.CardExistByNumNamePhone(input))
                return NotFound();

            var cards = _cardService.GetCardByNumNamePhone(input);

            return Ok(cards);
        }

        [HttpGet]
        public async Task<IActionResult> SortByDate(string dateFrom, string dateTo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cardService.CardExistByDate(dateFrom, dateTo))
                return NotFound();

            var cards = _mapper.Map<List<CardDTO>>(_cardService.SortCardByDate(dateFrom, dateTo));

            return Ok(cards);
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
                List<Combo> comboList = new List<Combo>();

                foreach (int comboId in cardDTO.ComboId)
                {
                    var combo = _cardService.GetCombo(comboId);
                    comboList.Add(combo);
                }

                existingCard.Combos = comboList;
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
    }
}
