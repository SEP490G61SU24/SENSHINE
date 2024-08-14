
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
         .Include(i => i.Cards)
         .Include(i => i.InvoiceCombos)
         .Include(i => i.InvoiceServices)
         .ToListAsync();

            return _mapper.Map<IEnumerable<InvoiceDTO>>(invoices);

            
        }

            public async Task<InvoiceDTO?> GetInvoiceDetail(int id)
            {
                var invoice = await _context.Invoices
                                            .Include(i => i.Customer)
                                            .Include(i => i.Promotion)
                                            .Include(i => i.Spa)
                                            .Include(i => i.Cards)
                                            .Include(i => i.InvoiceCombos)
                                            .Include(i => i.InvoiceServices)
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
                                            .Include(i => i.Cards)
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
        }
    }


