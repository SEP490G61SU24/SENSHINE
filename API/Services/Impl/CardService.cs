using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using API.Ultils;
using API.Dtos;
using AutoMapper;

namespace API.Services.Impl
{
    public class CardService : ICardService
    {
        private readonly SenShineSpaContext _context;

        public CardService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<Card> CreateCard(Card card)
        {
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();

            return card;
        }

        public ICollection<Card> GetCards()
        {
            return _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).Include(i => i.Invoices).ToList();
        }

        public Card GetCard(int id)
        {
            return _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).Include(i => i.Invoices).Where(c => c.Id == id).FirstOrDefault();
        }

        public ICollection<Card> GetCardByNumNamePhone(string input)
        {
            input = input.ToLower();
            var cards = _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).Include(i => i.Invoices);

            return cards.Where(c => c.CardNumber.ToLower().Contains(input)
                                 || (c.Customer.FirstName + " " + c.Customer.MidName + " " + c.Customer.LastName).ToLower().Contains(input)
                                 || c.Customer.Phone.Contains(input)).ToList();
        }

        public ICollection<Card> SortCardByDate(string dateFrom, string dateTo)
        {
            DateTime parsedDateFrom = FormatDateTimeUtils.ParseDateTimeLikeSSMS(dateFrom);
            DateTime parsedDateTo = FormatDateTimeUtils.ParseDateTimeLikeSSMS(dateTo);

            return _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).Include(i => i.Invoices).Where(c => c.CreateDate >= parsedDateFrom
                                                                                          && c.CreateDate <= parsedDateTo).ToList();
        }

        public async Task<Card> UpdateCard(int id, Card card)
        {
            var cardUpdate = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
            cardUpdate.CustomerId = card.CustomerId;
            cardUpdate.CreateDate = card.CreateDate;
            cardUpdate.Status = card.Status;
            await _context.SaveChangesAsync();

            return cardUpdate;
        }

        public async Task<Card> ActiveDeactiveCard(int id)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if (card.Status == "Active")
            {
                card.Status = "Deactive";
            }
            else
            {
                card.Status = "Active";
            }
            await _context.SaveChangesAsync();

            return card;
        }

        public bool CardExist(int id)
        {
            return _context.Cards.Any(c => c.Id == id);
        }

        public bool CardExistByNumNamePhone(string input)
        {
            input = input.ToLower();
            var cards = _context.Cards.Include(c => c.Customer);

            return cards.Any(c => c.CardNumber.ToLower().Contains(input)
                                 || (c.Customer.FirstName + " " + c.Customer.MidName + " " + c.Customer.LastName).ToLower().Contains(input)
                                 || c.Customer.Phone.Contains(input));
        }

        public bool CardExistByDate(string dateFrom, string dateTo)
        {
            DateTime parsedDateFrom = FormatDateTimeUtils.ParseDateTimeLikeSSMS(dateFrom);
            DateTime parsedDateTo = FormatDateTimeUtils.ParseDateTimeLikeSSMS(dateTo);

            return _context.Cards.Any(c => c.CreateDate <= parsedDateTo
                                        && c.CreateDate >= parsedDateFrom);
        }

        public ICollection<CardCombo> GetCardComboByCard(int id)
        {
            // Fetch all related card combos in one go
            var cardCombos = _context.CardCombos.Include(c => c.Combo).Where(c => c.CardId == id).ToList();

            return cardCombos;
        }


        public CardCombo GetCardCombo(int id)
        {
            return _context.CardCombos.Where(c => c.Id == id).FirstOrDefault();
        }

        public async Task<CardCombo> CreateCardCombo(CardCombo cardCombo)
        {
            await _context.CardCombos.AddAsync(cardCombo);
            await _context.SaveChangesAsync();

            return cardCombo;
        }
    }
}
