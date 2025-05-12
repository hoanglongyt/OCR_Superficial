using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Tesseract;
using UglyToad.PdfPig;

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
            Console.WriteLine($"TessDataPath: {_tessDataPath}");
        }

        // Endpoint tổng hợp để xử lý tất cả định dạng
        [HttpPost("extract-text")]
        public async Task<IActionResult> ExtractText(IFormFile file, [FromQuery] string language = "vie")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Please upload a file.");
                }

                string contentType = file.ContentType.ToLower();
                string fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (contentType == "application/pdf" || fileExtension == ".pdf")
                {
                    string extractedText = await ExtractTextFromPdf(file);
                    return Ok(new { Text = extractedText });
                }
                else if (contentType == "text/plain" || fileExtension == ".txt")
                {
                    string extractedText = await ExtractTextFromText(file);
                    return Ok(new { Text = extractedText });
                }
                else
                {
                    return BadRequest("Unsupported file format. Supported formats: pdf, text.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Phương thức xử lý ảnh (chỉ giữ lại phần OCR)
        private async Task<string> ExtractTextFromImageInternal(IFormFile imageFile, string language)
        {
            try
            {
                // Lấy ảnh đã được tiền xử lý từ PreprocessImageForOcr
                var preprocessResult = await PreprocessImageForOcr(imageFile);
                if (preprocessResult is FileContentResult fileResult)
                {
                    byte[] processedImageBytes = fileResult.FileContents;

                    if (!Directory.Exists(_tessDataPath))
                    {
                        throw new Exception($"Tessdata directory not found at: {_tessDataPath}");
                    }

                    var languageFile = Path.Combine(_tessDataPath, $"{language}.traineddata");
                    if (!System.IO.File.Exists(languageFile))
                    {
                        throw new Exception($"Language file not found: {languageFile}");
                    }

                    using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                    engine.SetVariable("tessedit_pageseg_mode", (int)PageSegMode.SingleBlock);

                    using var img = Pix.LoadFromMemory(processedImageBytes);
                    using var page = engine.Process(img);

                    return page.GetText();
                }
                else
                {
                    throw new Exception("Failed to preprocess image.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"OCR processing failed: {ex.Message}");
            }
        }

        // Phương thức xử lý PDF
        private async Task<string> ExtractTextFromPdf(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var document = PdfDocument.Open(memoryStream);
            var extractedText = new System.Text.StringBuilder();

            foreach (var page in document.GetPages())
            {
                var text = page.Text;
                extractedText.AppendLine(text);
            }

            return extractedText.ToString();
        }

        // Phương thức xử lý file text
        private async Task<string> ExtractTextFromText(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var reader = new StreamReader(memoryStream);
            return await reader.ReadToEndAsync();
        }

        // Endpoint resize ảnh (di chuyển phần resize từ ExtractTextFromImageInternal)
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

                // Resize ảnh (di chuyển từ ExtractTextFromImageInternal)
                int newWidth = image.Width * 2; // Có thể điều chỉnh tỷ lệ
                int newHeight = image.Height * 2;
                using var resizedImage = new Mat();
                CvInvoke.Resize(image, resizedImage, new System.Drawing.Size(newWidth, newHeight));

                using var resizedImageStream = new VectorOfByte();
                CvInvoke.Imencode(".jpg", resizedImage, resizedImageStream);
                byte[] resizedImageBytes = resizedImageStream.ToArray();

                return File(resizedImageBytes, "image/jpeg", "resized-image.jpg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint tiền xử lý ảnh cho OCR (di chuyển các bước preprocessing từ ExtractTextFromImageInternal)
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

                // Resize ảnh (di chuyển từ ExtractTextFromImageInternal)
                int newWidth = image.Width * 2;
                int newHeight = image.Height * 2;
                using var resizedImage = new Mat();
                CvInvoke.Resize(image, resizedImage, new System.Drawing.Size(newWidth, newHeight));

                // Chuyển sang grayscale
                using var grayImage = new Mat();
                CvInvoke.CvtColor(resizedImage, grayImage, ColorConversion.Bgr2Gray);

                // Tăng cường độ tương phản
                using var equalizedImage = new Mat();
                CvInvoke.EqualizeHist(grayImage, equalizedImage);

                // Áp dụng Gaussian Blur
                using var blurredImage = new Mat();
                CvInvoke.GaussianBlur(equalizedImage, blurredImage, new System.Drawing.Size(5, 5), 0);

                // Áp dụng Otsu thresholding
                using var thresholdImage = new Mat();
                CvInvoke.Threshold(blurredImage, thresholdImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

                // Loại bỏ nhiễu bằng Morphology
                using var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
                using var morphedImage = new Mat();
                CvInvoke.MorphologyEx(thresholdImage, morphedImage, MorphOp.Open, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                using var processedImageStream = new VectorOfByte();
                CvInvoke.Imencode(".bmp", morphedImage, processedImageStream); // Sử dụng .bmp cho Tesseract
                byte[] processedImageBytes = processedImageStream.ToArray();

                return File(processedImageBytes, "image/bmp", "processed-image.bmp");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint trích xuất văn bản từ ảnh (giữ nguyên)
        [HttpPost("extract-text-from-image")]
        public async Task<IActionResult> ExtractTextFromImage(IFormFile imageFile, [FromQuery] string language = "vie")
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("Please upload an image file.");
                }

                string contentType = imageFile.ContentType.ToLower();
                string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                if (contentType.Contains("image") || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".webp")
                {
                    string extractedText = await ExtractTextFromImageInternal(imageFile, language);
                    return Ok(new { Text = extractedText });
                }
                else
                {
                    return BadRequest("Unsupported file format. Supported formats: jpg, jpeg, png, webp.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}