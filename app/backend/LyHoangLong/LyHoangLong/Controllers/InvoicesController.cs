using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcrSystem.DataAccess;
using OcrSystem.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OcrSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly OcrDbContext _context;

        public InvoicesController(OcrDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return await _context.Invoices
                    .Where(i => i.UserID == userId)
                    .Include(i => i.User)
                    .Include(i => i.InvoiceItems)
                    .Include(i => i.InvoiceImages)
                    .Include(i => i.OCRResults)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoice = await _context.Invoices
                    .Include(i => i.User)
                    .Include(i => i.InvoiceItems)
                    .Include(i => i.InvoiceImages)
                    .Include(i => i.OCRResults)
                    .FirstOrDefaultAsync(i => i.InvoiceID == id && i.UserID == userId);

                if (invoice == null)
                {
                    return NotFound("Invoice not found or you do not have access to this invoice.");
                }

                return invoice;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                invoice.UserID = userId;
                invoice.Status = "Pending";

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceID }, invoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateInvoice(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceID)
            {
                return BadRequest("Invoice ID mismatch.");
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var existingInvoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceID == id && i.UserID == userId);

                if (existingInvoice == null)
                {
                    return NotFound("Invoice not found or you do not have access to this invoice.");
                }

                existingInvoice.Vendor = invoice.Vendor;
                existingInvoice.TotalAmount = invoice.TotalAmount;
                existingInvoice.InvoiceDate = invoice.InvoiceDate;
                existingInvoice.Status = invoice.Status;

                _context.Entry(existingInvoice).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceID == id && i.UserID == userId);

                if (invoice == null)
                {
                    return NotFound("Invoice not found or you do not have access to this invoice.");
                }

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}