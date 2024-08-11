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
        public async Task<ActionResult<Invoice>> CreateInvoice([FromBody] InvoiceDTO invoiceDto)
        {
            try
            {
                

                // Map DTO to Invoice entity
                var newInvoice = _mapper.Map<Invoice>(invoiceDto);
                
                // Save the invoice
                

                // Handle Cards
                if (invoiceDto.CardIds != null && invoiceDto.CardIds.Any())
                {
                    var existingCards = await _dbContext.Cards
                      .Where(c => invoiceDto.CardIds.Contains(c.Id))
                      .ToListAsync();

                    if (existingCards.Count != invoiceDto.CardIds.Count)
                    {
                        return BadRequest("Một hoặc nhiều thẻ không tồn tại.");
                    }

                    foreach (var card in existingCards)
                    {
                        newInvoice.Cards.Add(card);
                    }
                }

                // Handle Services
                if (invoiceDto.ServiceIds != null && invoiceDto.ServiceIds.Any())
                {
                    var existingServices = await _dbContext.Services
                      .Where(c => invoiceDto.ServiceIds.Contains(c.Id))
                      .ToListAsync();

                    if (existingServices.Count != invoiceDto.ServiceIds.Count)
                    {
                        return BadRequest("Một hoặc nhiều dịch vụ không tồn tại.");
                    }

                    foreach (var service in existingServices)
                    {
                        newInvoice.Services.Add(service);
                    }
                }

                // Handle Combos
                if (invoiceDto.ComboIds != null && invoiceDto.ComboIds.Any())
                {
                    var existingCombos = await _dbContext.Combos
                      .Where(c => invoiceDto.ComboIds.Contains(c.Id)) 
                      .ToListAsync();

                    if (existingCombos.Count != invoiceDto.ComboIds.Count)
                    {
                        return BadRequest("Một hoặc nhiều gói dịch vụ không tồn tại.");
                    }

                    foreach (var combo in existingCombos)
                    {
                        newInvoice.Combos.Add(combo);
                    }
                }
                _dbContext.Invoices.Add(newInvoice);
                await _dbContext.SaveChangesAsync();


                return CreatedAtAction(nameof(GetInvoice), new { id = newInvoice.Id }, newInvoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpPut("EditInvoice/{id}")]
        public async Task<ActionResult<Invoice>> EditInvoice(int id, [FromBody] InvoiceDTO invoiceDto)
        {
            try
            {
                // Retrieve the existing invoice with related entities
                var existingInvoice = await _dbContext.Invoices
                    .Include(i => i.Cards)
                    .Include(i => i.Services)
                    .Include(i => i.Combos)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (existingInvoice == null)
                {
                    return NotFound("Invoice not found.");
                }

                // Map basic properties from DTO to existing invoice
                _mapper.Map(invoiceDto, existingInvoice);

                // Update Cards
                if (invoiceDto.CardIds != null)
                {
                    var existingCards = await _dbContext.Cards
                        .Where(c => invoiceDto.CardIds.Contains(c.Id))
                        .ToListAsync();

                    if (existingCards.Count != invoiceDto.CardIds.Count)
                    {
                        return BadRequest("One or more cards do not exist.");
                    }

                    // Clear existing cards and add the new ones
                    existingInvoice.Cards.Clear();
                    foreach (var card in existingCards)
                    {
                        existingInvoice.Cards.Add(card);
                    }
                }

                // Update Services
                if (invoiceDto.ServiceIds != null)
                {
                    var existingServices = await _dbContext.Services
                        .Where(s => invoiceDto.ServiceIds.Contains(s.Id))
                        .ToListAsync();

                    if (existingServices.Count != invoiceDto.ServiceIds.Count)
                    {
                        return BadRequest("One or more services do not exist.");
                    }

                    // Clear existing services and add the new ones
                    existingInvoice.Services.Clear();
                    foreach (var service in existingServices)
                    {
                        existingInvoice.Services.Add(service);
                    }
                }

                // Update Combos
                if (invoiceDto.ComboIds != null)
                {
                    var existingCombos = await _dbContext.Combos
                        .Where(c => invoiceDto.ComboIds.Contains(c.Id))
                        .ToListAsync();

                    if (existingCombos.Count != invoiceDto.ComboIds.Count)
                    {
                        return BadRequest("One or more combos do not exist.");
                    }

                    // Clear existing combos and add the new ones
                    existingInvoice.Combos.Clear();
                    foreach (var combo in existingCombos)
                    {
                        existingInvoice.Combos.Add(combo);
                    }
                }

                // Save the updated invoice
                await _dbContext.SaveChangesAsync();

                // Return the updated invoice DTO
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
                    .Include(i => i.Services)
                    .Include(i => i.Combos)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (existingInvoice == null)
                {
                    return NotFound("Invoice not found.");
                }

                // Clear related entities (optional, depending on cascade delete configuration)
                existingInvoice.Cards.Clear();
                existingInvoice.Services.Clear();
                existingInvoice.Combos.Clear();

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



        [HttpGet("GetInvoiceByDate")]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoicesByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var invoices = await _invoiceService.InvoicesByDateRange(from, to);
            return Ok(invoices);
        }
    }
}
