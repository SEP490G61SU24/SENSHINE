using API.Dtos;
using API.Models;
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

        public async Task<NewsDTO> GetNewsDetail(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return null; 
            }

            return mapper.Map<NewsDTO>(news);
        }

        public async Task<IEnumerable<NewsDTORequest>> NewsByDate(DateTime From, DateTime To)
        {
            DateTime fromDate = From.Date;
            DateTime toDate = To.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


            var data = await _context.News
                .Where(x => x.PublishedDate >= fromDate && x.PublishedDate <= toDate)
                .ToListAsync();

            var result = mapper.Map<IEnumerable<NewsDTORequest>>(data);
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

    }
}
