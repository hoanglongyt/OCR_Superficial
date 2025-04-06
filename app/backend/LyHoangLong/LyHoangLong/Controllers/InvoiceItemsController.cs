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
    public class InvoiceItemsController : ControllerBase
    {
        private readonly OcrDbContext _context;

        public InvoiceItemsController(OcrDbContext context)
        {
            _context = context;
        }

        [HttpGet("{invoiceId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InvoiceItem>>> GetInvoiceItems(int invoiceId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceID == invoiceId && i.UserID == userId);

                if (invoice == null)
                {
                    return NotFound("Invoice not found or you do not have access to this invoice.");
                }

                return await _context.InvoiceItems
                    .Where(i => i.InvoiceID == invoiceId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Detail/{itemId}")]
        [Authorize]
        public async Task<ActionResult<InvoiceItem>> GetInvoiceItem(int itemId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoiceItem = await _context.InvoiceItems
                    .Include(i => i.Invoice)
                    .FirstOrDefaultAsync(i => i.ItemID == itemId);

                if (invoiceItem == null)
                {
                    return NotFound("Invoice item not found.");
                }

                if (invoiceItem.Invoice.UserID != userId)
                {
                    return Forbid("You do not have access to this invoice item.");
                }

                return Ok(invoiceItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<InvoiceItem>> CreateInvoiceItem(InvoiceItem item)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceID == item.InvoiceID && i.UserID == userId);

                if (invoice == null)
                {
                    return NotFound("Invoice not found or you do not have access to this invoice.");
                }

                _context.InvoiceItems.Add(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetInvoiceItem), new { itemId = item.ItemID }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{itemId}")]
        [Authorize]
        public async Task<IActionResult> UpdateInvoiceItem(int itemId, InvoiceItem item)
        {
            if (itemId != item.ItemID)
            {
                return BadRequest("Invoice item ID mismatch.");
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var existingItem = await _context.InvoiceItems
                    .Include(i => i.Invoice)
                    .FirstOrDefaultAsync(i => i.ItemID == itemId);

                if (existingItem == null)
                {
                    return NotFound("Invoice item not found.");
                }

                if (existingItem.Invoice.UserID != userId)
                {
                    return Forbid("You do not have access to this invoice item.");
                }

                existingItem.Description = item.Description;
                existingItem.Quantity = item.Quantity;
                existingItem.UnitPrice = item.UnitPrice;
                existingItem.TotalPrice = item.TotalPrice;

                _context.Entry(existingItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{itemId}")]
        [Authorize]
        public async Task<IActionResult> DeleteInvoiceItem(int itemId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoiceItem = await _context.InvoiceItems
                    .Include(i => i.Invoice)
                    .FirstOrDefaultAsync(i => i.ItemID == itemId);

                if (invoiceItem == null)
                {
                    return NotFound("Invoice item not found.");
                }

                if (invoiceItem.Invoice.UserID != userId)
                {
                    return Forbid("You do not have access to this invoice item.");
                }

                _context.InvoiceItems.Remove(invoiceItem);
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