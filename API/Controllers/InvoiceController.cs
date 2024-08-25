﻿using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var invoices = await _invoiceService.ListInvoices();
            return Ok(invoices);
        }


        [HttpGet("DetailInvoiceById")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoice(int id)
        {
            var invoice = await _invoiceService.GetInvoiceDetail(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }


        [HttpPost("AddInvoice")]
        public async Task<ActionResult<InvoiceDTO>> CreateInvoice([FromBody] InvoiceDTO invoiceDto)
        {
            try
            {
                // Map DTO to Invoice entity
                var newInvoice = _mapper.Map<Invoice>(invoiceDto);

                // Add the invoice to the context
                _dbContext.Invoices.Add(newInvoice);

                // Save changes to get the Id for the new invoice
                await _dbContext.SaveChangesAsync();

                // Handle Services
                if (invoiceDto.ServiceIds != null && invoiceDto.ServiceIds.Any())
                {
                    var serviceQuantities = invoiceDto.ServiceQuantities ?? new Dictionary<int, int?>();

                    foreach (var serviceId in invoiceDto.ServiceIds)
                    {
                        newInvoice.InvoiceServices.Add(new InvoiceService
                        {
                            InvoiceId = newInvoice.Id,
                            ServiceId = serviceId,
                            Quantity = serviceQuantities.ContainsKey(serviceId) ? serviceQuantities[serviceId] : null
                        });
                    }
                }

                // Handle Combos
                if (invoiceDto.ComboIds != null && invoiceDto.ComboIds.Any())
                {
                    var comboQuantities = invoiceDto.ComboQuantities ?? new Dictionary<int, int?>();

                    foreach (var comboId in invoiceDto.ComboIds)
                    {
                        newInvoice.InvoiceCombos.Add(new InvoiceCombo
                        {
                            InvoiceId = newInvoice.Id,
                            ComboId = comboId,
                            Quantity = comboQuantities.ContainsKey(comboId) ? comboQuantities[comboId] : null
                        });
                    }
                }

                // Save changes again to persist InvoiceServices and InvoiceCombos
                await _dbContext.SaveChangesAsync();

                // Map back to DTO to return
                var createdInvoiceDto = _mapper.Map<InvoiceDTO>(newInvoice);
                return CreatedAtAction(nameof(GetInvoice), new { id = newInvoice.Id }, createdInvoiceDto);
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging as needed)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }







        [HttpPut("EditInvoice/{id}")]
        public async Task<ActionResult<InvoiceDTO>> EditInvoice(int id, [FromBody] InvoiceDTO invoiceDto)
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

                // Map basic properties from DTO to existing invoice
                _mapper.Map(invoiceDto, existingInvoice);

                // Update Services
                existingInvoice.InvoiceServices.Clear();
                if (invoiceDto.ServiceIds != null)
                {
                    var serviceQuantities = invoiceDto.ServiceQuantities ?? new Dictionary<int, int?>();
                    foreach (var serviceId in invoiceDto.ServiceIds)
                    {
                        existingInvoice.InvoiceServices.Add(new InvoiceService
                        {
                            InvoiceId = existingInvoice.Id,
                            ServiceId = serviceId,
                            Quantity = serviceQuantities.ContainsKey(serviceId) ? serviceQuantities[serviceId] : null
                        });
                    }
                }

                // Update Combos
                existingInvoice.InvoiceCombos.Clear();
                if (invoiceDto.ComboIds != null)
                {
                    var comboQuantities = invoiceDto.ComboQuantities ?? new Dictionary<int, int?>();
                    foreach (var comboId in invoiceDto.ComboIds)
                    {
                        existingInvoice.InvoiceCombos.Add(new InvoiceCombo
                        {
                            InvoiceId = existingInvoice.Id,
                            ComboId = comboId,
                            Quantity = comboQuantities.ContainsKey(comboId) ? comboQuantities[comboId] : null
                        });
                    }
                }

                // Save the updated invoice
                await _dbContext.SaveChangesAsync();

                // Map back to DTO to return
                var updatedInvoiceDto = _mapper.Map<InvoiceDTO>(existingInvoice);
                return Ok(updatedInvoiceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UpdateInvoiceStatus")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id)
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


        [HttpGet("GetInvoiceByDate")]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoicesByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var invoices = await _invoiceService.InvoicesByDateRange(from, to);
            return Ok(invoices);
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
                default:
                    return BadRequest("Invalid period specified");
            }
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddTicks(-1);
            try
            {
                // Revenue Report
                var revenueReport = await _dbContext.Invoices
                    .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                    .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month, i.InvoiceDate.Day })
                    .OrderByDescending(g => g.Key.Year)
                    .ThenByDescending(g => g.Key.Month)
                    .ThenByDescending(g => g.Key.Day)
                    .Select(g => new RevenueReport
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        TotalRevenue = g.Sum(i => i.Amount) ?? 0
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
                    RevenueReports = revenueReport,
                    InvoiceStatusSummary = statusSummary,
                    ServiceSummaries = serviceSummary,
                    ComboSummaries = comboSummary
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, "Internal server error");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
