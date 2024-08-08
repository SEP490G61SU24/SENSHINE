using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<InvoiceDTO>> PostInvoice([FromBody] InvoiceDTO invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdInvoice = await _invoiceService.AddInvoice(invoiceDto);
            return CreatedAtAction(nameof(GetInvoice), new { id = createdInvoice.Id }, createdInvoice);
        }

        
        [HttpPut("EditInvoice")]
        public async Task<ActionResult<InvoiceDTO>> PutInvoice(int id, [FromBody] InvoiceDTO invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedInvoice = await _invoiceService.EditInvoice(id, invoiceDto);

            if (updatedInvoice == null)
            {
                return NotFound();
            }

            return Ok(updatedInvoice);
        }

        
        [HttpDelete("DeleteInvoice")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _invoiceService.DeleteInvoice(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        
        [HttpGet("GetInvoiceByDate")]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoicesByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var invoices = await _invoiceService.InvoicesByDateRange(from, to);
            return Ok(invoices);
        }
    }
}
