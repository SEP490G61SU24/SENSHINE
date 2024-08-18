using API.Dtos;
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
                    .Include(i => i.Cards)
                    .Include(i => i.InvoiceServices)
                    .Include(i => i.InvoiceCombos)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (existingInvoice == null)
                {
                    return NotFound("Invoice not found.");
                }

                // Clear related entities (optional, depending on cascade delete configuration)
                existingInvoice.Cards.Clear();
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
        public async Task<IActionResult> GetAllInvoicesPaging([FromQuery] int? idspa = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null,[FromQuery] string? status = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var pageData = await _invoiceService.GetInvoiceListBySpaId(idspa, pageIndex, pageSize, searchTerm, startDate, endDate,status);
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
    }
}
