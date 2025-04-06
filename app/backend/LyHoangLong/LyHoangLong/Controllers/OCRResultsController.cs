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
    public class OCRResultsController : ControllerBase
    {
        private readonly OcrDbContext _context;

        public OCRResultsController(OcrDbContext context)
        {
            _context = context;
        }

        [HttpGet("{invoiceId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OCRResult>>> GetOCRResults(int invoiceId)
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

                return await _context.OCRResults
                    .Where(o => o.InvoiceID == invoiceId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Detail/{ocrId}")]
        [Authorize]
        public async Task<ActionResult<OCRResult>> GetOCRResult(int ocrId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var ocrResult = await _context.OCRResults
                    .Include(o => o.Invoice)
                    .FirstOrDefaultAsync(o => o.OCRID == ocrId);

                if (ocrResult == null)
                {
                    return NotFound("OCR result not found.");
                }

                if (ocrResult.Invoice.UserID != userId)
                {
                    return Forbid("You do not have access to this OCR result.");
                }

                return Ok(ocrResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OCRResult>> CreateOCRResult(OCRResult ocrResult)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceID == ocrResult.InvoiceID && i.UserID == userId);

                if (invoice == null)
                {
                    return NotFound("Invoice not found or you do not have access to this invoice.");
                }

                ocrResult.ProcessedAt = DateTime.UtcNow;
                _context.OCRResults.Add(ocrResult);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOCRResult), new { ocrId = ocrResult.OCRID }, ocrResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{ocrId}")]
        [Authorize]
        public async Task<IActionResult> UpdateOCRResult(int ocrId, OCRResult ocrResult)
        {
            if (ocrId != ocrResult.OCRID)
            {
                return BadRequest("OCR result ID mismatch.");
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var existingResult = await _context.OCRResults
                    .Include(o => o.Invoice)
                    .FirstOrDefaultAsync(o => o.OCRID == ocrId);

                if (existingResult == null)
                {
                    return NotFound("OCR result not found.");
                }

                if (existingResult.Invoice.UserID != userId)
                {
                    return Forbid("You do not have access to this OCR result.");
                }

                existingResult.OCRText = ocrResult.OCRText;
                existingResult.Confidence = ocrResult.Confidence;
                existingResult.ProcessedAt = DateTime.UtcNow;

                _context.Entry(existingResult).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{ocrId}")]
        [Authorize]
        public async Task<IActionResult> DeleteOCRResult(int ocrId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var ocrResult = await _context.OCRResults
                    .Include(o => o.Invoice)
                    .FirstOrDefaultAsync(o => o.OCRID == ocrId);

                if (ocrResult == null)
                {
                    return NotFound("OCR result not found.");
                }

                if (ocrResult.Invoice.UserID != userId)
                {
                    return Forbid("You do not have access to this OCR result.");
                }

                _context.OCRResults.Remove(ocrResult);
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