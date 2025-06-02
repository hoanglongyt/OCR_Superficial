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

                    // Optimize Tesseract settings for better accuracy
                    ConfigureTesseractEngine(engine);

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

        // Advanced image preprocessing for OCR with multiple enhancement techniques
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

                // Apply advanced preprocessing pipeline
                var processedImage = ApplyAdvancedPreprocessing(image);

                using var processedImageStream = new VectorOfByte();
                CvInvoke.Imencode(".png", processedImage, processedImageStream); // Use PNG for better quality
                byte[] processedImageBytes = processedImageStream.ToArray();

                processedImage.Dispose();
                return File(processedImageBytes, "image/png", "processed-image.png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Advanced preprocessing pipeline with multiple enhancement techniques
        private Mat ApplyAdvancedPreprocessing(Mat originalImage)
        {
            // Step 1: Convert to grayscale
            using var grayImage = new Mat();
            CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);

            // Step 2: Intelligent resizing based on image dimensions
            var resizedImage = ApplyIntelligentResize(grayImage);

            // Step 3: Noise reduction using bilateral filter (preserves edges better than Gaussian)
            using var denoisedImage = new Mat();
            CvInvoke.BilateralFilter(resizedImage, denoisedImage, 9, 75, 75);

            // Step 4: Skew correction (deskewing)
            var deskewedImage = CorrectSkew(denoisedImage);

            // Step 5: Advanced contrast enhancement using CLAHE (Contrast Limited Adaptive Histogram Equalization)
            var enhancedImage = ApplyCLAHE(deskewedImage);

            // Step 6: Sharpening filter to enhance text edges
            var sharpenedImage = ApplyUnsharpMask(enhancedImage);

            // Step 7: Adaptive thresholding for better text separation
            var thresholdedImage = ApplyAdaptiveThresholding(sharpenedImage);

            // Step 8: Morphological operations to clean up text
            var cleanedImage = ApplyMorphologicalCleaning(thresholdedImage);

            // Step 9: Border padding for better OCR results
            var finalImage = AddBorderPadding(cleanedImage);

            // Dispose intermediate images
            resizedImage.Dispose();
            deskewedImage.Dispose();
            enhancedImage.Dispose();
            sharpenedImage.Dispose();
            thresholdedImage.Dispose();
            cleanedImage.Dispose();

            return finalImage;
        }

        // Intelligent resizing based on image characteristics
        private Mat ApplyIntelligentResize(Mat image)
        {
            var resizedImage = new Mat();

            // Calculate optimal size based on image dimensions and text size estimation
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Target DPI for OCR (300 DPI is optimal for Tesseract)
            double targetDPI = 300.0;
            double currentDPI = Math.Max(originalWidth, originalHeight) / 8.5; // Estimate current DPI

            double scaleFactor;
            if (currentDPI < 150)
            {
                // Low resolution image - scale up significantly
                scaleFactor = targetDPI / currentDPI;
                scaleFactor = Math.Min(scaleFactor, 4.0); // Cap at 4x to avoid excessive memory usage
            }
            else if (currentDPI > 400)
            {
                // High resolution image - scale down slightly
                scaleFactor = targetDPI / currentDPI;
                scaleFactor = Math.Max(scaleFactor, 0.5); // Don't scale down too much
            }
            else
            {
                // Good resolution - minimal scaling
                scaleFactor = 1.2;
            }

            int newWidth = (int)(originalWidth * scaleFactor);
            int newHeight = (int)(originalHeight * scaleFactor);

            // Use INTER_CUBIC for upscaling, INTER_AREA for downscaling
            var interpolation = scaleFactor > 1.0 ? Inter.Cubic : Inter.Area;

            CvInvoke.Resize(image, resizedImage, new System.Drawing.Size(newWidth, newHeight), 0, 0, interpolation);
            return resizedImage;
        }

        // Skew correction using Hough Line Transform
        private Mat CorrectSkew(Mat image)
        {
            var correctedImage = new Mat();

            try
            {
                // Detect edges for line detection
                using var edges = new Mat();
                CvInvoke.Canny(image, edges, 50, 150, 3);

                // Detect lines using Hough Transform
                using var lines = new VectorOfPointF();
                CvInvoke.HoughLines(edges, lines, 1, Math.PI / 180, 100);

                if (lines.Size > 0)
                {
                    // Calculate average angle of detected lines
                    double angleSum = 0;
                    int validLines = 0;

                    for (int i = 0; i < Math.Min(lines.Size, 10); i++) // Use first 10 lines
                    {
                        var line = lines[i];
                        double angle = line.Y; // Theta value

                        // Convert to degrees and normalize
                        double angleDegrees = angle * 180.0 / Math.PI;
                        if (angleDegrees > 90) angleDegrees -= 180;

                        // Only consider small angles (likely text lines)
                        if (Math.Abs(angleDegrees) < 45)
                        {
                            angleSum += angleDegrees;
                            validLines++;
                        }
                    }

                    if (validLines > 0)
                    {
                        double averageAngle = angleSum / validLines;

                        // Only correct if angle is significant enough
                        if (Math.Abs(averageAngle) > 0.5)
                        {
                            // Create rotation matrix
                            var center = new System.Drawing.PointF(image.Width / 2.0f, image.Height / 2.0f);
                            using var rotationMatrix = new Mat();
                            CvInvoke.GetRotationMatrix2D(center, averageAngle, 1.0, rotationMatrix);

                            // Apply rotation
                            CvInvoke.WarpAffine(image, correctedImage, rotationMatrix, image.Size, Inter.Cubic, Warp.Default, BorderType.Constant, new MCvScalar(255));
                            return correctedImage;
                        }
                    }
                }
            }
            catch
            {
                // If skew correction fails, return original image
            }

            // If no correction needed or failed, return copy of original
            image.CopyTo(correctedImage);
            return correctedImage;
        }

        // Enhanced contrast using adaptive histogram equalization
        private Mat ApplyCLAHE(Mat image)
        {
            var enhancedImage = new Mat();

            // Use regular histogram equalization as fallback if CLAHE is not available
            // This provides similar contrast enhancement
            CvInvoke.EqualizeHist(image, enhancedImage);

            // Apply additional contrast enhancement using convertScaleAbs
            using var contrastEnhanced = new Mat();
            CvInvoke.ConvertScaleAbs(enhancedImage, contrastEnhanced, 1.2, 10); // alpha=1.2 (contrast), beta=10 (brightness)

            contrastEnhanced.CopyTo(enhancedImage);
            return enhancedImage;
        }

        // Unsharp mask for text sharpening
        private Mat ApplyUnsharpMask(Mat image)
        {
            var sharpenedImage = new Mat();

            // Create Gaussian blur
            using var blurred = new Mat();
            CvInvoke.GaussianBlur(image, blurred, new System.Drawing.Size(0, 0), 1.0);

            // Create unsharp mask
            using var mask = new Mat();
            CvInvoke.Subtract(image, blurred, mask);

            // Apply unsharp mask
            CvInvoke.AddWeighted(image, 1.5, mask, 0.5, 0, sharpenedImage);

            return sharpenedImage;
        }

        // Adaptive thresholding for better text separation
        private Mat ApplyAdaptiveThresholding(Mat image)
        {
            var thresholdedImage = new Mat();

            // Try multiple thresholding methods and choose the best one
            using var otsuThreshold = new Mat();
            using var adaptiveThreshold = new Mat();

            // Otsu thresholding
            CvInvoke.Threshold(image, otsuThreshold, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

            // Adaptive thresholding
            CvInvoke.AdaptiveThreshold(image, adaptiveThreshold, 255, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 11, 2);

            // Combine both methods using weighted average
            CvInvoke.AddWeighted(otsuThreshold, 0.7, adaptiveThreshold, 0.3, 0, thresholdedImage);

            // Ensure binary output
            CvInvoke.Threshold(thresholdedImage, thresholdedImage, 127, 255, ThresholdType.Binary);

            return thresholdedImage;
        }

        // Advanced morphological operations for text cleaning
        private Mat ApplyMorphologicalCleaning(Mat image)
        {
            var cleanedImage = new Mat();

            // Remove small noise with opening operation
            using var openKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new System.Drawing.Size(2, 2), new System.Drawing.Point(-1, -1));
            using var opened = new Mat();
            CvInvoke.MorphologyEx(image, opened, MorphOp.Open, openKernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            // Close small gaps in text with closing operation
            using var closeKernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(2, 1), new System.Drawing.Point(-1, -1));
            CvInvoke.MorphologyEx(opened, cleanedImage, MorphOp.Close, closeKernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            return cleanedImage;
        }

        // Add border padding for better OCR results
        private Mat AddBorderPadding(Mat image)
        {
            var paddedImage = new Mat();

            // Add 20 pixel white border on all sides
            int borderSize = 20;
            CvInvoke.CopyMakeBorder(image, paddedImage, borderSize, borderSize, borderSize, borderSize, BorderType.Constant, new MCvScalar(255));

            return paddedImage;
        }

        // Configure Tesseract engine for optimal OCR performance
        private void ConfigureTesseractEngine(TesseractEngine engine)
        {
            // Page segmentation mode - Use single uniform block for invoices
            engine.SetVariable("tessedit_pageseg_mode", (int)PageSegMode.SingleBlock);

            // OCR Engine Mode - Use LSTM for better Vietnamese character recognition
            engine.SetVariable("tessedit_ocr_engine_mode", "1"); // LSTM only for better Vietnamese

            // Vietnamese-specific character whitelist for invoices
            // Include Vietnamese diacritics and common invoice characters
            engine.SetVariable("tessedit_char_whitelist",
                "0123456789" +
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                "ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚÝàáâãèéêìíòóôõùúýĂăĐđĨĩŨũƠơƯưẠạẢảẤấẦầẨẩẪẫẬậẮắẰằẲẳẴẵẶặẸẹẺẻẼẽẾếỀềỂểỄễỆệỈỉỊịỌọỎỏỐốỒồỔổỖỗỘộỚớỜờỞởỠỡỢợỤụỦủỨứỪừỬửỮữỰự" +
                ".,!?@#$%^&*()_+-=[]{}|;':\"<>?/~` ");

            // Improve word recognition for Vietnamese
            engine.SetVariable("tessedit_enable_dict_correction", "1");
            engine.SetVariable("tessedit_enable_bigram_correction", "1");

            // Preserve interword spaces (important for Vietnamese)
            engine.SetVariable("preserve_interword_spaces", "1");

            // Lower rejection threshold for Vietnamese characters
            engine.SetVariable("tessedit_reject_mode", "0");
            engine.SetVariable("tessedit_char_unblacklist", "");

            // Improve handling of Vietnamese text layout
            engine.SetVariable("textord_really_old_xheight", "0");
            engine.SetVariable("textord_min_xheight", "8");

            // Better handling of Vietnamese punctuation and diacritics
            engine.SetVariable("tessedit_load_sublangs", "1");
            engine.SetVariable("language_model_penalty_non_dict_word", "0.5");
            engine.SetVariable("language_model_penalty_non_freq_dict_word", "0.1");

            // Improve accuracy for Vietnamese fonts
            engine.SetVariable("classify_enable_learning", "1");
            engine.SetVariable("classify_enable_adaptive_matcher", "1");

            // Better handling of Vietnamese word boundaries
            engine.SetVariable("wordrec_enable_assoc", "1");
            engine.SetVariable("segment_penalty_dict_nonword", "1.0");

            // Optimize for invoice-like documents
            engine.SetVariable("tessedit_write_images", "0");
            engine.SetVariable("user_defined_dpi", "300");
        }

        // Enhanced OCR endpoint with preprocessing strategy selection
        [HttpPost("extract-text-enhanced")]
        public async Task<IActionResult> ExtractTextEnhanced(IFormFile imageFile, [FromQuery] string language = "vie", [FromQuery] string strategy = "advanced")
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("Please upload an image file.");
                }

                string contentType = imageFile.ContentType.ToLower();
                string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!contentType.Contains("image") && !new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".tiff" }.Contains(fileExtension))
                {
                    return BadRequest("Unsupported file format. Supported formats: jpg, jpeg, png, webp, bmp, tiff.");
                }

                // Apply preprocessing based on strategy
                byte[] processedImageBytes;
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                using var image = new Mat();
                CvInvoke.Imdecode(imageBytes, ImreadModes.Color, image);
                if (image.IsEmpty)
                {
                    return BadRequest("Failed to load image.");
                }

                Mat processedImage;
                switch (strategy.ToLower())
                {
                    case "simple":
                        processedImage = ApplySimplePreprocessing(image);
                        break;
                    case "document":
                        processedImage = ApplyDocumentPreprocessing(image);
                        break;
                    case "handwritten":
                        processedImage = ApplyHandwrittenPreprocessing(image);
                        break;
                    case "receipt":
                        processedImage = ApplyReceiptPreprocessing(image);
                        break;
                    case "advanced":
                    default:
                        processedImage = ApplyAdvancedPreprocessing(image);
                        break;
                }

                using var processedImageStream = new VectorOfByte();
                CvInvoke.Imencode(".png", processedImage, processedImageStream);
                processedImageBytes = processedImageStream.ToArray();
                processedImage.Dispose();

                // Perform OCR
                if (!Directory.Exists(_tessDataPath))
                {
                    throw new Exception($"Tessdata directory not found at: {_tessDataPath}");
                }

                // Validate language files for mixed languages
                ValidateLanguageFiles(language);

                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                ConfigureTesseractEngine(engine);

                using var img = Pix.LoadFromMemory(processedImageBytes);
                using var page = engine.Process(img);

                var extractedText = page.GetText();
                var confidence = page.GetMeanConfidence();

                return Ok(new {
                    Text = extractedText,
                    Confidence = confidence,
                    Strategy = strategy,
                    Language = language
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Simple preprocessing for high-quality images
        private Mat ApplySimplePreprocessing(Mat originalImage)
        {
            using var grayImage = new Mat();
            CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);

            var thresholdedImage = new Mat();
            CvInvoke.Threshold(grayImage, thresholdedImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

            return thresholdedImage;
        }

        // Document-specific preprocessing
        private Mat ApplyDocumentPreprocessing(Mat originalImage)
        {
            using var grayImage = new Mat();
            CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);

            // Resize for better OCR
            var resizedImage = ApplyIntelligentResize(grayImage);

            // Skew correction
            var deskewedImage = CorrectSkew(resizedImage);

            // CLAHE for contrast enhancement
            var enhancedImage = ApplyCLAHE(deskewedImage);

            // Adaptive thresholding
            var thresholdedImage = ApplyAdaptiveThresholding(enhancedImage);

            // Add padding
            var finalImage = AddBorderPadding(thresholdedImage);

            resizedImage.Dispose();
            deskewedImage.Dispose();
            enhancedImage.Dispose();
            thresholdedImage.Dispose();

            return finalImage;
        }

        // Handwritten text preprocessing
        private Mat ApplyHandwrittenPreprocessing(Mat originalImage)
        {
            using var grayImage = new Mat();
            CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);

            // Resize significantly for handwritten text
            var resizedImage = new Mat();
            CvInvoke.Resize(grayImage, resizedImage, new System.Drawing.Size(grayImage.Width * 3, grayImage.Height * 3), 0, 0, Inter.Cubic);

            // Strong noise reduction
            using var denoisedImage = new Mat();
            CvInvoke.BilateralFilter(resizedImage, denoisedImage, 15, 100, 100);

            // Gentle sharpening
            var sharpenedImage = ApplyUnsharpMask(denoisedImage);

            // Adaptive thresholding with larger kernel
            var thresholdedImage = new Mat();
            CvInvoke.AdaptiveThreshold(sharpenedImage, thresholdedImage, 255, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 21, 10);

            // Minimal morphological operations to preserve handwritten strokes
            using var kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new System.Drawing.Size(1, 1), new System.Drawing.Point(-1, -1));
            var cleanedImage = new Mat();
            CvInvoke.MorphologyEx(thresholdedImage, cleanedImage, MorphOp.Close, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            // Add padding
            var finalImage = AddBorderPadding(cleanedImage);

            resizedImage.Dispose();
            sharpenedImage.Dispose();
            thresholdedImage.Dispose();
            cleanedImage.Dispose();

            return finalImage;
        }

        // Receipt-specific preprocessing - optimized for clean printed receipts
        private Mat ApplyReceiptPreprocessing(Mat originalImage)
        {
            using var grayImage = new Mat();
            CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);

            // Enhance contrast specifically for receipts
            var enhancedImage = new Mat();
            CvInvoke.CLAHE(grayImage, 2.0, new System.Drawing.Size(8, 8), enhancedImage);

            // Light denoising - receipts usually have good quality
            var denoisedImage = new Mat();
            CvInvoke.MedianBlur(enhancedImage, denoisedImage, 3);

            // Simple binary threshold - receipts usually have good contrast
            var thresholdedImage = new Mat();
            CvInvoke.Threshold(denoisedImage, thresholdedImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

            // Light morphological cleaning to connect broken characters
            using var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(1, 1), new System.Drawing.Point(-1, -1));
            var cleanedImage = new Mat();
            CvInvoke.MorphologyEx(thresholdedImage, cleanedImage, MorphOp.Close, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            // Add padding
            var finalImage = AddBorderPadding(cleanedImage);

            enhancedImage.Dispose();
            denoisedImage.Dispose();
            thresholdedImage.Dispose();
            cleanedImage.Dispose();

            return finalImage;
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

        // Diagnostic endpoint to help troubleshoot OCR accuracy issues
        [HttpPost("diagnose-ocr")]
        public async Task<IActionResult> DiagnoseOCR(IFormFile imageFile, [FromQuery] string language = "vie")
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("Please upload an image file.");
                }

                var diagnostics = new
                {
                    ImageInfo = new
                    {
                        FileName = imageFile.FileName,
                        Size = imageFile.Length,
                        ContentType = imageFile.ContentType
                    },
                    TesseractInfo = new
                    {
                        TessDataPath = _tessDataPath,
                        TessDataExists = Directory.Exists(_tessDataPath),
                        RequestedLanguage = language,
                        AvailableLanguages = GetAvailableLanguages(),
                        LanguageValidation = ValidateLanguageFilesForDiagnostic(language)
                    },
                    ProcessingResults = new Dictionary<string, object>()
                };

                // Test image loading
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                using var image = new Mat();
                CvInvoke.Imdecode(imageBytes, ImreadModes.Color, image);

                var imageAnalysis = new
                {
                    LoadedSuccessfully = !image.IsEmpty,
                    Dimensions = new { Width = image.Width, Height = image.Height },
                    Channels = image.NumberOfChannels,
                    EstimatedDPI = Math.Max(image.Width, image.Height) / 8.5,
                    RecommendedStrategy = GetRecommendedStrategy(image)
                };

                ((Dictionary<string, object>)diagnostics.ProcessingResults)["ImageAnalysis"] = imageAnalysis;

                if (!image.IsEmpty)
                {
                    // Test different preprocessing strategies
                    var strategies = new[] { "simple", "document", "handwritten", "receipt", "advanced" };
                    var strategyResults = new Dictionary<string, object>();

                    foreach (var strategy in strategies)
                    {
                        try
                        {
                            Mat processedImage;
                            switch (strategy)
                            {
                                case "simple":
                                    processedImage = ApplySimplePreprocessing(image);
                                    break;
                                case "document":
                                    processedImage = ApplyDocumentPreprocessing(image);
                                    break;
                                case "handwritten":
                                    processedImage = ApplyHandwrittenPreprocessing(image);
                                    break;
                                case "receipt":
                                    processedImage = ApplyReceiptPreprocessing(image);
                                    break;
                                case "advanced":
                                default:
                                    processedImage = ApplyAdvancedPreprocessing(image);
                                    break;
                            }

                            using var processedImageStream = new VectorOfByte();
                            CvInvoke.Imencode(".png", processedImage, processedImageStream);
                            byte[] processedImageBytes = processedImageStream.ToArray();

                            // Quick OCR test
                            using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                            ConfigureTesseractEngine(engine);
                            using var img = Pix.LoadFromMemory(processedImageBytes);
                            using var page = engine.Process(img);

                            var text = page.GetText();
                            var confidence = page.GetMeanConfidence();

                            strategyResults[strategy] = new
                            {
                                ProcessedImageSize = processedImageBytes.Length,
                                ProcessedDimensions = new { Width = processedImage.Width, Height = processedImage.Height },
                                TextLength = text.Length,
                                Confidence = confidence,
                                TextPreview = text.Length > 100 ? text.Substring(0, 100) + "..." : text,
                                HasText = !string.IsNullOrWhiteSpace(text)
                            };

                            processedImage.Dispose();
                        }
                        catch (Exception ex)
                        {
                            strategyResults[strategy] = new { Error = ex.Message };
                        }
                    }

                    ((Dictionary<string, object>)diagnostics.ProcessingResults)["StrategyComparison"] = strategyResults;
                }

                return Ok(diagnostics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Diagnostic failed: {ex.Message}");
            }
        }

        // Validate language files for single or mixed languages
        private void ValidateLanguageFiles(string language)
        {
            var languages = language.Split('+');
            foreach (var lang in languages)
            {
                var languageFile = Path.Combine(_tessDataPath, $"{lang.Trim()}.traineddata");
                if (!System.IO.File.Exists(languageFile))
                {
                    throw new Exception($"Language file not found: {languageFile}. Available languages: {GetAvailableLanguages()}");
                }
            }
        }

        // Get list of available language files
        private string GetAvailableLanguages()
        {
            try
            {
                var files = Directory.GetFiles(_tessDataPath, "*.traineddata")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToArray();
                return string.Join(", ", files);
            }
            catch
            {
                return "Unable to list available languages";
            }
        }

        // Validate language files for diagnostic (returns info instead of throwing)
        private object ValidateLanguageFilesForDiagnostic(string language)
        {
            var languages = language.Split('+');
            var results = new List<object>();

            foreach (var lang in languages)
            {
                var languageFile = Path.Combine(_tessDataPath, $"{lang.Trim()}.traineddata");
                results.Add(new
                {
                    Language = lang.Trim(),
                    FilePath = languageFile,
                    Exists = System.IO.File.Exists(languageFile)
                });
            }

            return results;
        }

        // Helper method to recommend best strategy based on image characteristics
        private string GetRecommendedStrategy(Mat image)
        {
            if (image.IsEmpty) return "unknown";

            var estimatedDPI = Math.Max(image.Width, image.Height) / 8.5;

            if (estimatedDPI < 100)
                return "handwritten"; // Very low resolution, likely handwritten or poor quality
            else if (estimatedDPI < 200)
                return "advanced"; // Low resolution, needs full processing
            else if (estimatedDPI > 400)
                return "simple"; // High resolution, minimal processing needed
            else
                return "document"; // Good resolution, document-optimized processing
        }
    }
}