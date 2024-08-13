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
            try
            {
                await _context.Cards.AddAsync(card);
                await _context.SaveChangesAsync();

                return card;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating card.", ex);
            }
        }

        public ICollection<Card> GetCards()
        {
            try
            {
                return _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).Include(i => i.Invoices).ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error retrieving cards.", ex);
            }
        }

        public Card GetCard(int id)
        {
            try
            {
                return _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).Include(i => i.Invoices)
                                      .Where(c => c.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving card with ID {id}.", ex);
            }
        }

        public ICollection<Card> GetCardByNumNamePhone(string input)
        {
            try
            {
                input = input.ToLower();
                var cards = _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).Include(i => i.Invoices);

                return cards.Where(c => c.CardNumber.ToLower().Contains(input)
                                     || (c.Customer.FirstName + " " + c.Customer.MidName + " " + c.Customer.LastName)
                                        .ToLower().Contains(input)
                                     || c.Customer.Phone.Contains(input)).ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error searching cards by number, name, or phone.", ex);
            }
        }

        public async Task<Card> UpdateCard(int id, Card card)
        {
            try
            {
                var cardUpdate = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
                cardUpdate.CustomerId = card.CustomerId;
                cardUpdate.Status = card.Status;
                await _context.SaveChangesAsync();

                return cardUpdate;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error updating card with ID {id}.", ex);
            }
        }

        public async Task<Card> ActiveDeactiveCard(int id)
        {
            try
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
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error activating/deactivating card with ID {id}.", ex);
            }
        }

        public bool CardExist(int id)
        {
            try
            {
                return _context.Cards.Any(c => c.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error checking existence of card with ID {id}.", ex);
            }
        }

        public bool CardExistByNumNamePhone(string input)
        {
            try
            {
                input = input.ToLower();
                var cards = _context.Cards.Include(c => c.Customer);

                return cards.Any(c => c.CardNumber.ToLower().Contains(input)
                                     || (c.Customer.FirstName + " " + c.Customer.MidName + " " + c.Customer.LastName)
                                        .ToLower().Contains(input)
                                     || c.Customer.Phone.Contains(input));
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error checking card existence by number, name, or phone.", ex);
            }
        }

        public ICollection<CardCombo> GetCardComboByCard(int id)
        {
            try
            {
                // Fetch all related card combos in one go
                var cardCombos = _context.CardCombos.Include(c => c.Combo).Where(c => c.CardId == id).ToList();

                return cardCombos;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving card combos for card ID {id}.", ex);
            }
        }

        public CardCombo GetCardCombo(int id)
        {
            try
            {
                return _context.CardCombos.Where(c => c.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving card combo with ID {id}.", ex);
            }
        }

        public async Task<CardCombo> CreateCardCombo(CardCombo cardCombo)
        {
            try
            {
                await _context.CardCombos.AddAsync(cardCombo);
                await _context.SaveChangesAsync();

                return cardCombo;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating card combo.", ex);
            }
        }
    }
}
