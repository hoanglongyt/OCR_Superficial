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
            Console.WriteLine($"TessDataPath: {_tessDataPath}"); // In đường dẫn để kiểm tra
        }

        // Endpoint tổng hợp để xử lý tất cả định dạng
        [HttpPost("extract-text")]
        public async Task<IActionResult> ExtractText(IFormFile file, [FromQuery] string language = "eng")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Please upload a file.");
                }

                // Xác định định dạng file dựa trên ContentType hoặc phần mở rộng
                string contentType = file.ContentType.ToLower();
                string fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (contentType.Contains("image") || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".webp")
                {
                    // Xử lý file ảnh
                    string extractedText = await ExtractTextFromImageInternal(file, language);
                    return Ok(new { Text = extractedText });
                }
                else if (contentType == "application/pdf" || fileExtension == ".pdf")
                {
                    // Xử lý file PDF
                    string extractedText = await ExtractTextFromPdf(file);
                    return Ok(new { Text = extractedText });
                }
                else if (contentType == "text/plain" || fileExtension == ".txt")
                {
                    // Xử lý file text
                    string extractedText = await ExtractTextFromText(file);
                    return Ok(new { Text = extractedText });
                }
                else
                {
                    return BadRequest("Unsupported file format. Supported formats: image (jpg, png, jpeg, .webp), pdf, text.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Phương thức xử lý ảnh (dùng EmguCV để tiền xử lý)
        private async Task<string> ExtractTextFromImageInternal(IFormFile imageFile, string language)
        {
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            using var image = new Mat();
            CvInvoke.Imdecode(imageBytes, ImreadModes.Color, image);
            if (image.IsEmpty)
            {
                throw new Exception("Failed to load image.");
            }

            // Resize image
            int newWidth = image.Width * 2;
            int newHeight = image.Height * 2;
            using var resizedImage = new Mat();
            CvInvoke.Resize(image, resizedImage, new System.Drawing.Size(newWidth, newHeight));

            // Convert to grayscale
            using var grayImage = new Mat();
            CvInvoke.CvtColor(resizedImage, grayImage, ColorConversion.Bgr2Gray);

            // Enhance contrast
            using var equalizedImage = new Mat();
            CvInvoke.EqualizeHist(grayImage, equalizedImage);

            // Apply Gaussian Blur
            using var blurredImage = new Mat();
            CvInvoke.GaussianBlur(equalizedImage, blurredImage, new System.Drawing.Size(5, 5), 0);

            // Apply Otsu thresholding
            using var thresholdImage = new Mat();
            CvInvoke.Threshold(blurredImage, thresholdImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

            // Remove noise using Morphology
            using var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            using var morphedImage = new Mat();
            CvInvoke.MorphologyEx(thresholdImage, morphedImage, MorphOp.Open, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            using var processedImageStream = new VectorOfByte();
            CvInvoke.Imencode(".bmp", morphedImage, processedImageStream);
            byte[] processedImageBytes = processedImageStream.ToArray();

            if (!Directory.Exists(_tessDataPath))
            {
                throw new Exception($"Tessdata directory not found at: {_tessDataPath}");
            }

            var languageFile = Path.Combine(_tessDataPath, $"{language}.traineddata");
            if (!System.IO.File.Exists(languageFile))
            {
                throw new Exception($"Language file not found: {languageFile}");
            }

            using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.LstmOnly);
            engine.SetVariable("tessedit_pageseg_mode", (int)PageSegMode.SingleBlock);

            using var img = Pix.LoadFromMemory(processedImageBytes);
            using var page = engine.Process(img);

            return page.GetText();
        }

        // Phương thức xử lý PDF (không cần EmguCV)
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

                // Nếu PDF chứa ảnh, có thể chuyển thành ảnh và OCR (tùy chọn nâng cao)
                // Hiện tại chỉ trích xuất văn bản trực tiếp từ PDF
            }

            return extractedText.ToString();
        }

        // Phương thức xử lý file text (không cần EmguCV)
        private async Task<string> ExtractTextFromText(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var reader = new StreamReader(memoryStream);
            return await reader.ReadToEndAsync();
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

                using var processedImageStream = new VectorOfByte();
                CvInvoke.Imencode(".jpg", thresholdImage, processedImageStream);
                byte[] processedImageBytes = processedImageStream.ToArray();

                return File(processedImageBytes, "image/jpeg", "processed-image.jpg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Giữ lại endpoint ExtractTextFromImage để tương thích với các client cũ
        [HttpPost("extract-text-from-image")]
        public async Task<IActionResult> ExtractTextFromImage(IFormFile imageFile, [FromQuery] string language = "eng")
        {
            try
            {
                string extractedText = await ExtractTextFromImageInternal(imageFile, language);
                return Ok(new { Text = extractedText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}