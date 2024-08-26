using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface ICardService
    {
        Task<Card> CreateCard(Card card);
        Task<PaginatedList<CardDTO>> GetCards(int pageIndex, int pageSize, string searchTerm, string spaId);
        List<Card> GetAllCards();
        Card GetCard(int id);
        ICollection<Card> GetCardByNumNamePhone(string input);
        Task<Card> UpdateCard(int id, Card card);
        Task<Card> ActiveDeactiveCard(int id);
        Task<CardComboDTO> UseCardCombo(int id);
        bool CardExist(int id);
        bool CardExistByNumNamePhone(string input);
        ICollection<CardCombo> GetCardComboByCard(int id);
        ICollection<CardInvoice> GetCardInvoiceByCard(int id);
        CardCombo GetCardCombo(int id);
        Task<CardCombo> CreateCardCombo(CardCombo cardCombo);
        Task<CardInvoice> CreateCardInvoice(CardInvoice cardInvoice);
        Task<InvoiceDTO?> GetInvoiceById(int id);
    }
}
