using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace API.Services.Impl
{
    public class NewsService : INewsService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper mapper;
        public NewsService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        public async Task<News> AddNews(NewsDTO newsDto)
        {
            var news = mapper.Map<News>(newsDto);
            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return mapper.Map<News>(news);
        }

        public async Task<News> EditNews(int id, NewsDTO newsDto)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return null; 
            }

            mapper.Map(newsDto, news);
            _context.News.Update(news);
            await _context.SaveChangesAsync();

            return mapper.Map<News>(news);
        }

        public async Task<IEnumerable<NewsDTO>> ListNews()
        {
            var data = await _context.News.ToListAsync();
            return mapper.Map<IEnumerable<NewsDTO>>(data);
        }
        public async Task<IEnumerable<NewsDTO>> ListNewsSortByNewDate()
        {
            var data = await _context.News
                .OrderByDescending(news => news.PublishedDate)
                .ToListAsync();

            return mapper.Map<IEnumerable<NewsDTO>>(data);
        }


        public async Task<NewsDTO> GetNewsDetail(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return null; 
            }

            return mapper.Map<NewsDTO>(news);
        }

        public async Task<IEnumerable<NewsDTO>> NewsByDate(DateTime From, DateTime To)
        {
            DateTime fromDate = From.Date;
            DateTime toDate = To.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


            var data = await _context.News
                .Where(x => x.PublishedDate >= fromDate && x.PublishedDate <= toDate)
                .ToListAsync();

            var result = mapper.Map<IEnumerable<NewsDTO>>(data);
            return result;
        }

        public async Task<bool> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return false; 
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<FilteredPaginatedList<NewsDTO>> GetNews(int pageIndex = 1, int pageSize = 10, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Tạo query cơ bản
            IQueryable<News> query = _context.News.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.PublishedDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.PublishedDate <= endDate);
            }

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Title.Contains(searchTerm) ||
                                         u.Content.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var news = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var newsDtos = mapper.Map<IEnumerable<NewsDTO>>(news);

            return new FilteredPaginatedList<NewsDTO>
            {
                Items = newsDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }
    }
}
