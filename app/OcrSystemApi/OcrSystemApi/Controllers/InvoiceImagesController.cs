using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcrSystemApi.DataAccess;
using OcrSystemApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OcrSystemApiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceImagesController : ControllerBase
    {
        private readonly OcrDbContext _context;
        private readonly string _imageStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

        public InvoiceImagesController(OcrDbContext context)
        {
            _context = context;
            if (!Directory.Exists(_imageStoragePath))
            {
                Directory.CreateDirectory(_imageStoragePath);
            }
        }

        [HttpGet("{userid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InvoiceImage>>> GetInvoiceImages(int userId)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (currentUserId != userId)
                {
                    return Forbid("You do not have access to these images.");
                }

                var invoiceImages = await _context.InvoiceImages
                    .Where(i => i.UserID == userId)
                    .ToListAsync();

                return Ok(invoiceImages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Detail/{imageid}")]
        [Authorize]
        public async Task<ActionResult<InvoiceImage>> GetInvoiceImage(int imageId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoiceImage = await _context.InvoiceImages
                    .Include(i => i.Users)
                    .FirstOrDefaultAsync(i => i.ImageID == imageId);

                if (invoiceImage == null)
                {
                    return NotFound("Image not found.");
                }

                if (invoiceImage.UserID != userId)
                {
                    return Forbid("You do not have access to this image.");
                }

                return Ok(invoiceImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Upload/{imageid}")]
        [Authorize]
        public async Task<ActionResult<InvoiceImage>> UploadImage(int imageId, IFormFile file)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoiceImage = await _context.InvoiceImages
                    .FirstOrDefaultAsync(i => i.UserID == userId);


                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_imageStoragePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                invoiceImage = new InvoiceImage // Corrected variable usage
                {
                    UserID = userId,
                    ImageURL = $"/images/{fileName}",
                    UploadedAt = DateTime.UtcNow
                };

                _context.InvoiceImages.Add(invoiceImage);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetInvoiceImage), new { imageId = invoiceImage.ImageID }, invoiceImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{imageid}")]
        [Authorize]
        public async Task<IActionResult> DeleteInvoiceImage(int imageId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var invoiceImage = await _context.InvoiceImages
                    .FirstOrDefaultAsync(i => i.ImageID == imageId);

                if (invoiceImage == null)
                {
                    return NotFound("Image not found.");
                }

                if (invoiceImage.UserID != userId)
                {
                    return Forbid("You do not have access to this image.");
                }

                var filePath = Path.Combine(_imageStoragePath, Path.GetFileName(invoiceImage.ImageURL));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.InvoiceImages.Remove(invoiceImage);
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