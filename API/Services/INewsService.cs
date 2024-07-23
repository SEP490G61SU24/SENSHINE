using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface INewsService
    {
        Task<News> AddNews(NewsDTO newsDto);
        Task<News> EditNews(int id, NewsDTO newsDto);
        Task<IEnumerable<NewsDTO>> ListNews();
        Task<NewsDTO> GetNewsDetail(int id);
        Task<IEnumerable<NewsDTORequest>> NewsByDate(DateTime From, DateTime To);
        Task<bool> DeleteNews(int id);
    }
}
