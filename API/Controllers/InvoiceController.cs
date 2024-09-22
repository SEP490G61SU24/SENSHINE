using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;
        private readonly SenShineSpaContext _dbContext;

        public InvoiceController(SenShineSpaContext dbContext, IInvoiceService invoiceService, IMapper mapper)
        {
            _dbContext = dbContext;
            _invoiceService = invoiceService;
            _mapper = mapper;
        }


        [HttpGet("ListInvoice")]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoices()
        {
            try
            {
                var invoices = await _invoiceService.ListInvoices();
                return Ok(invoices);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }


        [HttpGet("DetailInvoiceById")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoice(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceDetail(id);

                if (invoice == null)
                {
                    return NotFound();
                }

                return Ok(invoice);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }
        [HttpGet("DetailInvoiceByIdUserandDate")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoiceDetailByIdUseranDate(int iduser, DateTime date)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceDetailbByUserIdandDdate(iduser,date);

                if (invoice == null)
                {
                    return NotFound();
                }

                return Ok(invoice);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }


        [HttpPost("AddInvoice")]
        public async Task<ActionResult<InvoiceDTO>> CreateInvoice([FromBody] InvoiceDTO invoiceDto)
        {
            try
            {
                var createdInvoiceDto = await _invoiceService.AddInvoice(invoiceDto);
                return CreatedAtAction(nameof(GetInvoice), new { id = createdInvoiceDto.Id }, createdInvoiceDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpPut("EditInvoice/{id}")]
        public async Task<ActionResult<InvoiceDTO>> EditInvoice(int id, [FromBody] InvoiceDTO invoiceDto)
        {
            try
            {
                var updatedInvoiceDto = await _invoiceService.EditInvoice(id, invoiceDto);
                return Ok(updatedInvoiceDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }



        [HttpDelete("DeleteInvoice/{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                // Retrieve the existing invoice with related entities
                var existingInvoice = await _dbContext.Invoices
                    .Include(i => i.InvoiceServices)
                    .Include(i => i.InvoiceCombos)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (existingInvoice == null)
                {
                    return NotFound("Invoice not found.");
                }

                // Clear related entities (optional, depending on cascade delete configuration)
                existingInvoice.InvoiceServices.Clear();
                existingInvoice.InvoiceCombos.Clear();

                // Remove the invoice from the database
                _dbContext.Invoices.Remove(existingInvoice);
                await _dbContext.SaveChangesAsync();

                return NoContent(); // Return 204 No Content on successful deletion
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpPut("UpdateInvoiceStatus")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id)
        {
            try
            {
                var invoice = await _dbContext.Invoices.FindAsync(id);

                if (invoice == null)
                {
                    return NotFound("Invoice not found.");
                }
                string status = "Paid";
                invoice.Status = status;

                _dbContext.Invoices.Update(invoice);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Invoice status updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }


        [HttpGet("GetInvoiceByDate")]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoicesByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var invoices = await _invoiceService.InvoicesByDateRange(from, to);
                return Ok(invoices);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }
        [HttpGet("GetInvoicesPaging")]
        public async Task<IActionResult> GetAllInvoicesPaging([FromQuery] int? idspa = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string? status = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var pageData = await _invoiceService.GetInvoiceListBySpaId(idspa, pageIndex, pageSize, searchTerm, startDate, endDate, status);
                return Ok(pageData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }
        [HttpGet("report-summary")]
        public async Task<IActionResult> GetReportSummary(string? period=null)
        {
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;

            switch (period.ToLower())
            {
                case "7days":
                    startDate = endDate.AddDays(-7);
                    break;
                case "1month":
                    startDate = endDate.AddMonths(-1);
                    break;
                case "4months":
                    startDate = endDate.AddMonths(-4);
                    break;
                case "1year":
                    startDate = endDate.AddYears(-1);
                    break;
                case "100years":
                    startDate = endDate.AddYears(-100);
                    break;
                default:
                    return BadRequest("Invalid period specified");
            }
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddTicks(-1);
            try
            {
                // Revenue Report
                // Step 1: Calculate revenue for each invoice
                var invoiceRevenues = await _dbContext.Invoices
                    .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                    .Select(i => new
                    {
                        Date = i.InvoiceDate,
                        ServiceRevenue = i.InvoiceServices.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)),
                        ComboRevenue = i.InvoiceCombos.Sum(ic => (ic.Price ?? 0) * (ic.Quantity ?? 0))
                    })
                    .ToListAsync(); // Execute the query and bring the results into memory

                // Step 2: Aggregate revenues by date
                var revenueReport = invoiceRevenues
                    .GroupBy(r => new { r.Date.Year, r.Date.Month, r.Date.Day })
                    .OrderByDescending(g => g.Key.Year)
                    .ThenByDescending(g => g.Key.Month)
                    .ThenByDescending(g => g.Key.Day)
                    .Select(g => new RevenueReport
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        TotalRevenue = g.Sum(r => r.ServiceRevenue + r.ComboRevenue)
                    })
                    .ToList(); 




                var discountedRevenue = await _dbContext.Invoices
                    .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                    .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month, i.InvoiceDate.Day })
                    .OrderByDescending(g => g.Key.Year)
                    .ThenByDescending(g => g.Key.Month)
                    .ThenByDescending(g => g.Key.Day)
                    .Select(g => new DiscountRevenueReport
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        discountRevenue = g.Sum(i => i.Amount) ?? 0
                    })
                    .ToListAsync();

                // Invoice Status Summary
                var statusSummary = await _dbContext.Invoices
                    .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                    .GroupBy(i => i.Status)
                    .Select(g => new RevenueReport
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Invoice Service Summary
                var serviceSummary = await _dbContext.Invoices
                    .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                    .SelectMany(i => i.InvoiceServices)
                    .GroupBy(i => new { i.ServiceId, i.Service.ServiceName })
                    .Select(g => new ServiceSummary
                    {
                        ServiceId = g.Key.ServiceId,
                        ServiceName = g.Key.ServiceName,
                        TotalQuantity = g.Sum(i => i.Quantity.GetValueOrDefault())
                    })
                    .ToListAsync();

                // Invoice Combo Summary
                var comboSummary = await _dbContext.Invoices
                    .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                    .SelectMany(i => i.InvoiceCombos)
                    .GroupBy(ic => new { ic.ComboId, ic.Combo.Name })
                    .Select(g => new ComboSummary
                    {
                        ComboId = g.Key.ComboId,
                        ComboName = g.Key.Name,
                        TotalQuantity = g.Sum(ic => ic.Quantity.GetValueOrDefault())
                    })
                    .ToListAsync();

                // Combine results into the view model
                var result = new CombinedReportDTO
                {
                    DiscountRevenueReports= discountedRevenue,
                    RevenueReports = revenueReport,
                    InvoiceStatusSummary = statusSummary,
                    ServiceSummaries = serviceSummary,
                    ComboSummaries = comboSummary
                };

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }


        [HttpGet("daily-revenue")]
        public async Task<IActionResult> GetDailyRevenueForCurrentMonth()
        {
            try
            {
                var (labels, values) = await _invoiceService.GetDailyRevenueForCurrentMonth();
                return Ok(new { Labels = labels, Values = values });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

    }
}
