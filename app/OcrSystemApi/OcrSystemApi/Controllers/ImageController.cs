using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Tesseract;

namespace OcrSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly string _tessDataPath;

        public ImageController(IWebHostEnvironment environment)
        {
            _tessDataPath = Path.Combine(environment.ContentRootPath, "TesseractData");
        }

        [HttpPost("resize-image")]
        public async Task<IActionResult> ResizeImage(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("Please upload an image file.");
                }

                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                using var image = new Mat();
                CvInvoke.Imdecode(imageBytes, ImreadModes.Color, image);
                if (image.IsEmpty)
                {
                    return BadRequest("Failed to load image.");
                }

                int newWidth = image.Width / 2;
                int newHeight = image.Height / 2;
                using var resizedImage = new Mat();
                CvInvoke.Resize(image, resizedImage, new System.Drawing.Size(newWidth, newHeight));

                byte[] resizedImageBytes = CvInvoke.Imencode(".jpg", resizedImage); // Thay ToJpegData
                return File(resizedImageBytes, "image/jpeg", "resized-image.jpg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("preprocess-image-for-ocr")]
        public async Task<IActionResult> PreprocessImageForOcr(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("Please upload an image file.");
                }

                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                using var image = new Mat();
                CvInvoke.Imdecode(imageBytes, ImreadModes.Color, image);
                if (image.IsEmpty)
                {
                    return BadRequest("Failed to load image.");
                }

                using var grayImage = new Mat();
                CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

                using var blurredImage = new Mat();
                CvInvoke.GaussianBlur(grayImage, blurredImage, new System.Drawing.Size(5, 5), 0);

                using var thresholdImage = new Mat();
                CvInvoke.Threshold(blurredImage, thresholdImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

                byte[] processedImageBytes = CvInvoke.Imencode(".jpg", thresholdImage); // Thay ToJpegData
                return File(processedImageBytes, "image/jpeg", "processed-image.jpg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("extract-text-from-image")]
        public async Task<IActionResult> ExtractTextFromImage(IFormFile imageFile, [FromQuery] string language = "eng")
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("Please upload an image file.");
                }

                // Đọc file ảnh vào bộ nhớ
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                // Load ảnh bằng EmguCV
                using var image = new Mat();
                CvInvoke.Imdecode(imageBytes, ImreadModes.Color, image);
                if (image.IsEmpty)
                {
                    return BadRequest("Failed to load image.");
                }

                // Tiền xử lý ảnh: chuyển sang grayscale, làm nét
                using var grayImage = new Mat();
                CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

                using var blurredImage = new Mat();
                CvInvoke.GaussianBlur(grayImage, blurredImage, new System.Drawing.Size(5, 5), 0);

                using var thresholdImage = new Mat();
                CvInvoke.Threshold(blurredImage, thresholdImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

                // Chuyển ảnh sang định dạng byte (dạng BMP) để Tesseract xử lý
                byte[] processedImageBytes = CvInvoke.Imencode(".bmp", thresholdImage); // Sửa cú pháp Imencode

                // Khởi tạo Tesseract engine
                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                using var img = Pix.LoadFromMemory(processedImageBytes);
                using var page = engine.Process(img);

                // Trích xuất văn bản
                var extractedText = page.GetText();

                // Trả về văn bản đã trích xuất
                return Ok(new { Text = extractedText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}