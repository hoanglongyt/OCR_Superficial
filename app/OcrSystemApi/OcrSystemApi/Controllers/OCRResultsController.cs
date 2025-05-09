using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcrSystemApi.DataAccess;
using OcrSystemApi.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OcrSystemApi.Controllers
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

        [HttpGet("{imageid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OCRResult>>> GetOCRResults(int imageId) // Fixed parameter name
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoice = await _context.InvoiceImages
                    .FirstOrDefaultAsync(i => i.ImageID == imageId && i.UserID == userId);

                if (invoice == null)
                {
                    return NotFound("Invoice not found or you do not have access to this invoice.");
                }

                return await _context.OCRResults
                    .Where(o => o.ImageID == imageId && o.InvoiceImages.UserID == userId) // Adjusted to use the correct navigation property
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("detail/{ocrid}")]
        [Authorize]
        public async Task<ActionResult<OCRResult>> GetOCRResult(int ocrId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var ocrResult = await _context.OCRResults
                    .Include(o => o.InvoiceImages)
                    .FirstOrDefaultAsync(o => o.OCRID == ocrId);

                if (ocrResult == null)
                {
                    return NotFound("OCR result not found.");
                }

                if (ocrResult.InvoiceImages.UserID != userId) // Corrected to check user access
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
                var invoice = await _context.InvoiceImages
                    .FirstOrDefaultAsync(i => i.ImageID == ocrResult.ImageID && i.UserID == userId);

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

        [HttpPut("{ocrid}")]
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
                    .Include(o => o.InvoiceImages)
                    .FirstOrDefaultAsync(o => o.OCRID == ocrId);

                if (existingResult == null)
                {
                    return NotFound("OCR result not found.");
                }

                if (existingResult.InvoiceImages.UserID != userId)
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

        [HttpDelete("{ocrid}")]
        [Authorize]
        public async Task<IActionResult> DeleteOCRResult(int ocrId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var ocrResult = await _context.OCRResults
                    .Include(o => o.InvoiceImages)
                    .FirstOrDefaultAsync(o => o.OCRID == ocrId);

                if (ocrResult == null)
                {
                    return NotFound("OCR result not found.");
                }

                if (ocrResult.InvoiceImages.UserID != userId)
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