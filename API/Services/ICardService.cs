using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface ICardService
    {
        Task<Card> CreateCard(Card card);
        ICollection<Card> GetCards();
        Card GetCard(int id);
        ICollection<Card> GetCardByNumNamePhone(string input);
        Task<Card> UpdateCard(int id, Card card);
        Task<Card> ActiveDeactiveCard(int id);
        bool CardExist(int id);
        bool CardExistByNumNamePhone(string input);
        ICollection<CardCombo> GetCardComboByCard(int id);
        CardCombo GetCardCombo(int id);
        Task<CardCombo> CreateCardCombo(CardCombo cardCombo);
    }
}
