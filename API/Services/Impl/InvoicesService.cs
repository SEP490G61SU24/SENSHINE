
using API.Ultils;
using AutoMapper;
using global::API.Dtos;
using global::API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class InvoicesService : IInvoiceService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public InvoicesService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<InvoiceDTO> AddInvoice(InvoiceDTO invoiceDto)
        {
            var invoice = _mapper.Map<Invoice>(invoiceDto);
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return _mapper.Map<InvoiceDTO>(invoice);
        }

        public async Task<InvoiceDTO> EditInvoice(int id, InvoiceDTO invoiceDto)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return null;
            }

            _mapper.Map(invoiceDto, invoice);
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return _mapper.Map<InvoiceDTO>(invoice);
        }

        public async Task<IEnumerable<InvoiceDTO>> ListInvoices()
        {
            var invoices = await _context.Invoices
         .Include(i => i.Customer)
         .Include(i => i.Promotion)
         .Include(i => i.Spa)
         .Include(i => i.InvoiceCombos)
         .Include(i => i.InvoiceServices)
         .ToListAsync();

            return _mapper.Map<IEnumerable<InvoiceDTO>>(invoices);


        }
        public async Task<FilteredPaginatedList<InvoiceDTO>> GetInvoiceListBySpaId(int? spaId = null, int pageIndex = 1, int pageSize = 10, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null, string? status = null)
        {
            // Tạo query cơ bản
            IQueryable<Invoice> query = _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Promotion)
                .Include(i => i.Spa)
                .Include(i => i.InvoiceCombos)
                .Include(i => i.InvoiceServices).AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.InvoiceDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.InvoiceDate <= endDate);
            }
            if (spaId.HasValue)
            {
                query = query.Where(x => x.SpaId == spaId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.Status.Contains(status));
            }
            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Description.Contains(searchTerm) ||
                                         u.Customer.FirstName.Contains(searchTerm) ||
                                         u.Customer.LastName.Contains(searchTerm) ||
                                         u.Customer.MidName.Contains(searchTerm) ||
                                         u.Promotion.PromotionName.Contains(searchTerm) ||
                                         u.Spa.SpaName.Contains(searchTerm) ||
                                         u.Amount.ToString().Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var news = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var newsDtos = _mapper.Map<IEnumerable<InvoiceDTO>>(news);

            return new FilteredPaginatedList<InvoiceDTO>
            {
                Items = newsDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }
        public async Task<InvoiceDTO?> GetInvoiceDetail(int id)
        {
            var invoice = await _context.Invoices
                                        .Include(i => i.Customer)
                                        .Include(i => i.Promotion)
                                        .Include(i => i.Spa)
                                        .Include(i => i.InvoiceCombos).ThenInclude(i => i.Combo)
                                        .Include(i => i.InvoiceServices).ThenInclude(i => i.Service)
                                        .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return null;
            }

            return _mapper.Map<InvoiceDTO>(invoice);
        }

        public async Task<IEnumerable<InvoiceDTO>> InvoicesByDateRange(DateTime from, DateTime to)
        {
            DateTime fromDate = from.Date;
            DateTime toDate = to.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var invoices = await _context.Invoices
                                        .Include(i => i.Customer)
                                        .Include(i => i.Promotion)
                                        .Include(i => i.Spa)
                                        .Include(i => i.InvoiceCombos)
                                        .Include(i => i.InvoiceServices)
                                         .Where(x => x.InvoiceDate >= fromDate && x.InvoiceDate <= toDate)
                                         .ToListAsync();

            return _mapper.Map<IEnumerable<InvoiceDTO>>(invoices);
        }

        public async Task<bool> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return false;
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<(IEnumerable<string> Labels, IEnumerable<decimal> Values)> GetDailyRevenueForCurrentMonth()
        {
            var currentDate = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var dailyRevenue = await _context.Invoices
                .Where(i => i.InvoiceDate >= firstDayOfMonth && i.InvoiceDate <= lastDayOfMonth)
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(i => i.Amount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var labels = dailyRevenue.Select(x => x.Date.ToString("dd-MM-yyyy")).ToArray();
            var values = dailyRevenue.Select(x => x.TotalAmount ?? 0).ToArray();

            return (labels, values);
        }

    }

}

