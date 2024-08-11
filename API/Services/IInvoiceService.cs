using API.Dtos;
using API.Models;
using API.Services.Impl;

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
    }
    }

