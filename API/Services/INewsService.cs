using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface INewsService
    {
        Task<News> AddNews(NewsDTO newsDto);
        Task<News> EditNews(int id, NewsDTO newsDto);
        Task<IEnumerable<NewsDTO>> ListNews();
        Task<PaginatedList<NewsDTO>> GetNews(int pageIndex, int pageSize, string searchTerm);
        Task<IEnumerable<NewsDTO>> ListNewsSortByNewDate();
        Task<NewsDTO> GetNewsDetail(int id);
        Task<IEnumerable<NewsDTO>> NewsByDate(DateTime From, DateTime To);
        Task<bool> DeleteNews(int id);
    }
}
