using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using API.Ultils;
using API.Dtos;
using AutoMapper;
using System;

namespace API.Services.Impl
{
    public class CardService : ICardService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public CardService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async Task<PaginatedList<CardDTO>> GetCards(int pageIndex, int pageSize, string searchTerm, string spaId)
        {
            // Tạo query cơ bản
            IQueryable<Card> query = _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos);

            int? spaIdInt = spaId != null && spaId != "ALL"
            ? int.Parse(spaId)
            : (int?)null;

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.BranchId == spaIdInt.Value);
            }

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.CardNumber.Contains(searchTerm) ||
                                         c.Customer.FirstName.Contains(searchTerm) ||
                                         c.Customer.MidName.Contains(searchTerm) ||
                                         c.Customer.LastName.Contains(searchTerm) ||
                                         c.Customer.Phone.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var cards = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var cardDtos = _mapper.Map<IEnumerable<CardDTO>>(cards);

            return new PaginatedList<CardDTO>
            {
                Items = cardDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }

        public Card GetCard(int id)
        {
            try
            {
                return _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos)
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
                var cards = _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos);

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
                                     || (c.Customer.FirstName + " " + c.Customer.MidName + " " + c.Customer.LastName).ToLower().Contains(input)
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

        public List<Card> GetAllCards()
        {
            try
            {
                return _context.Cards.Include(c => c.Customer).Include(c => c.CardCombos).ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error retrieving cards.", ex);
            }
        }

        public ICollection<CardInvoice> GetCardInvoiceByCard(int id)
        {
            try
            {
                // Fetch all related card combos in one go
                var invoices = _context.CardInvoices.Include(i => i.Invoice).Where(i => i.CardId == id).ToList();

                return invoices;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving card combos for card ID {id}.", ex);
            }
        }

        public async Task<CardInvoice> CreateCardInvoice(CardInvoice cardInvoice)
        {
            try
            {
                await _context.CardInvoices.AddAsync(cardInvoice);
                await _context.SaveChangesAsync();

                return cardInvoice;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating card combo.", ex);
            }
        }

        public async Task<InvoiceDTO?> GetInvoiceById(int id)
        {
            try
            {
                var invoice = await _context.Invoices.Include(i => i.Customer).FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                {
                    return null;
                }

                return _mapper.Map<InvoiceDTO>(invoice);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating card combo.", ex);
            }
        }
    }
}
