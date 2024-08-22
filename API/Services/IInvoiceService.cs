using API.Dtos;
using API.Models;
using API.Services.Impl;
using API.Ultils;

namespace API.Services
{
    public interface IInvoiceService 
    {
        Task<InvoiceDTO> AddInvoice(InvoiceDTO invoiceDto);
        Task<InvoiceDTO> EditInvoice(int id, InvoiceDTO invoiceDto);
        Task<IEnumerable<InvoiceDTO>> ListInvoices();
        Task<InvoiceDTO?> GetInvoiceDetail(int id);
        Task<IEnumerable<InvoiceDTO>> InvoicesByDateRange(DateTime from, DateTime to);
        Task<bool> DeleteInvoice(int id);
        Task<FilteredPaginatedList<InvoiceDTO>> GetInvoiceListBySpaId(int? spaId = null, int pageIndex = 1, int pageSize = 10, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null, string? status = null);
        Task<(IEnumerable<string> Labels, IEnumerable<decimal> Values)> GetDailyRevenueForCurrentMonth();

    }
    }

