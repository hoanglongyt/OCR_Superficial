using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UglyToad.PdfPig;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocWord = DocumentFormat.OpenXml.Wordprocessing;
using System.IO.Compression;

namespace OcrSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfConverterController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _pdfiumPath;

        public PdfConverterController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _pdfiumPath = Path.Combine(_environment.ContentRootPath, "pdfium.dll");
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertPdf(IFormFile file, [FromQuery] string format)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a PDF file.");

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only PDF files are allowed.");

            try
            {
                switch (format.ToLower())
                {
                    case "word":
                    case "docx":
                        return await PdfToWord(file);
                    case "png":
                        return await PdfToPng(file);
                    case "jpg":
                    case "jpeg":
                        return await PdfToJpg(file);
                    case "excel":
                    case "xlsx":
                        return await PdfToExcel(file);
                    case "pptx":
                        return await PdfToPptx(file);
                    case "text":
                    case "txt":
                        return await PdfToText(file);
                    case "tiff":
                        return await PdfToTiff(file);
                    case "html":
                        return await PdfToHtml(file);
                    case "eps":
                        return await PdfToEps(file);
                    case "webp":
                        return await PdfToWebp(file);
                    case "epub":
                        return await PdfToEpub(file);
                    case "svg":
                        return await PdfToSvg(file);
                    case "dxf":
                        return await PdfToDxf(file);
                    case "mobi":
                        return await PdfToMobi(file);
                    case "azw3":
                        return await PdfToAzw3(file);
                    case "image":
                        return await PdfToImage(file);
                    default:
                        return BadRequest($"Unsupported format: {format}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Conversion failed: {ex.Message}");
            }
        }

        [HttpPost("pdf-to-word")]
        public async Task<IActionResult> PdfToWord(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Tạo file Word mới
            var outputStream = new MemoryStream();
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document))
            {
                // Thêm main document part
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new DocWord.Document(); // No ambiguity here
                Body body = mainPart.Document.AppendChild(new Body());

                // Đọc văn bản từ PDF
                using (var pdfDocument = UglyToad.PdfPig.PdfDocument.Open(memoryStream))
                {
                    foreach (var page in pdfDocument.GetPages())
                    {
                        var text = page.Text;

                        // Thêm văn bản vào Word document
                        var para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(text)));
                        body.AppendChild(para);
                    }
                }

                mainPart.Document.Save();
            }

            outputStream.Position = 0;
            return File(outputStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "converted.docx");
        }

        [HttpPost("pdf-to-png")]
        public async Task<IActionResult> PdfToPng(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var document = PdfiumViewer.PdfDocument.Load(memoryStream);
            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    using var image = document.Render(i, 300, 300, true);

                    using var imageStream = new MemoryStream();
                    image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                    imageStream.Position = 0;

                    var entry = archive.CreateEntry($"page_{i + 1}.png", CompressionLevel.Fastest);
                    using var entryStream = entry.Open();
                    await imageStream.CopyToAsync(entryStream);
                }
            }

            zipStream.Position = 0;
            return File(zipStream.ToArray(), "application/zip", "pdf_pages.zip");
        }


        [HttpPost("pdf-to-jpg")]
        public async Task<IActionResult> PdfToJpg(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var document = PdfiumViewer.PdfDocument.Load(memoryStream);
            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    using var image = document.Render(i, 300, 300, true);

                    using var imageStream = new MemoryStream();
                    image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageStream.Position = 0;

                    var entry = archive.CreateEntry($"page_{i + 1}.jpg", CompressionLevel.Fastest);
                    using var entryStream = entry.Open();
                    await imageStream.CopyToAsync(entryStream);
                }
            }

            zipStream.Position = 0;
            return File(zipStream.ToArray(), "application/zip", "pdf_pages.zip");
        }


        [HttpPost("pdf-to-excel")]
        public async Task<IActionResult> PdfToExcel(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            // Đọc văn bản từ PDF và thêm vào Excel
            using (var pdfDocument = UglyToad.PdfPig.PdfDocument.Open(memoryStream))
            {
                int rowIndex = 1;

                foreach (var page in pdfDocument.GetPages())
                {
                    var lines = page.Text.Split('\n');

                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            worksheet.Cell(rowIndex, 1).Value = line;
                            rowIndex++;
                        }
                    }
                }
            }

            var outputStream = new MemoryStream();
            workbook.SaveAs(outputStream);
            outputStream.Position = 0;

            return File(outputStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "converted.xlsx");
        }

        [HttpPost("pdf-to-pptx")]
        public async Task<IActionResult> PdfToPptx(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Tạo file PowerPoint
            using var pptxStream = new MemoryStream();
            using (var presentation = PresentationDocument.Create(pptxStream, PresentationDocumentType.Presentation))
            {
                var presentationPart = presentation.AddPresentationPart();
                presentationPart.Presentation = new Presentation();

                var slidePartList = new List<SlidePart>();
                var slideIdList = new SlideIdList();
                uint slideId = 256;

                using var pdfDoc = PdfiumViewer.PdfDocument.Load(memoryStream);

                for (int i = 0; i < pdfDoc.PageCount; i++)
                {
                    // Trích xuất văn bản từ trang i
                    string text = pdfDoc.GetPdfText(i);

                    // Tạo slide mới
                    var slidePart = presentationPart.AddNewPart<SlidePart>();
                    slidePart.Slide = new Slide(new CommonSlideData(new ShapeTree()));

                    // Tạo shape chứa nội dung
                    var shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;

                    // Bắt buộc phải có placeholder shape đầu tiên (Title shape)
                    var nonVisualShapeProps = new NonVisualShapeProperties(
                        new NonVisualDrawingProperties() { Id = 1, Name = "Title" },
                        new NonVisualShapeDrawingProperties(new DocumentFormat.OpenXml.Drawing.ShapeLocks() { NoGrouping = true }),
                        new ApplicationNonVisualDrawingProperties());

                    var shapeProperties = new ShapeProperties();

                    var textBody = new TextBody(
                        new DocumentFormat.OpenXml.Drawing.BodyProperties(),
                        new DocumentFormat.OpenXml.Drawing.ListStyle(),
                        new DocumentFormat.OpenXml.Drawing.Paragraph(
                            new DocumentFormat.OpenXml.Drawing.Run(
                                new DocumentFormat.OpenXml.Drawing.Text(text ?? ""))
                        ));

                    var shape = new Shape(nonVisualShapeProps, shapeProperties, textBody);

                    shapeTree.AppendChild(shape);

                    // Gán slide ID
                    slideIdList.Append(new SlideId()
                    {
                        Id = slideId++,
                        RelationshipId = presentationPart.GetIdOfPart(slidePart)
                    });

                    slidePartList.Add(slidePart);
                }

                presentationPart.Presentation.Append(slideIdList);
                presentationPart.Presentation.Save();
            }

            pptxStream.Position = 0;
            return File(pptxStream.ToArray(), "application/vnd.openxmlformats-officedocument.presentationml.presentation", "converted.pptx");
        }


        [HttpPost("pdf-to-text")]
        public async Task<IActionResult> PdfToText(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var stringBuilder = new StringBuilder();

            using (var pdfDocument = UglyToad.PdfPig.PdfDocument.Open(memoryStream))
            {
                foreach (var page in pdfDocument.GetPages())
                {
                    stringBuilder.AppendLine(page.Text);
                }
            }

            var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            return File(outputStream.ToArray(), "text/plain", "converted.txt");
        }

        [HttpPost("pdf-to-html")]
        public async Task<IActionResult> PdfToHtml(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var htmlContent = new StringBuilder();
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head><title>Converted PDF</title></head>");
            htmlContent.AppendLine("<body>");

            using (var pdfDocument = UglyToad.PdfPig.PdfDocument.Open(memoryStream))
            {
                foreach (var page in pdfDocument.GetPages())
                {
                    var text = page.Text;
                    htmlContent.AppendLine("<div class='page'>");
                    htmlContent.AppendLine($"<p>{page.Text.Replace(Environment.NewLine, "<br>")}</p>");
                    htmlContent.AppendLine("</div>");
                    htmlContent.AppendLine("<hr>");
                }
            }

            htmlContent.AppendLine("</body></html>");

            var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent.ToString()));
            return File(outputStream.ToArray(), "text/html", "converted.html");
        }

        [HttpPost("pdf-to-tiff")]
        public async Task<IActionResult> PdfToTiff(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Sử dụng thư viện để render PDF sang image
            using var outputStream = new MemoryStream();
            using (var pdfiumDocument = PdfiumViewer.PdfDocument.Load(memoryStream))
            {
                // Lấy trang đầu tiên
                using var bitmap = pdfiumDocument.Render(0, 300, 300, true);
                bitmap.Save(outputStream, System.Drawing.Imaging.ImageFormat.Tiff);
            }

            outputStream.Position = 0;
            return File(outputStream.ToArray(), "image/tiff", "converted.tiff");
        }

        [HttpPost("pdf-to-eps")]
        public async Task<IActionResult> PdfToEps(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var epsHeader = new StringBuilder();
            epsHeader.AppendLine("%!PS-Adobe-3.0 EPSF-3.0");
            epsHeader.AppendLine("%%Creator: PDF Converter");
            epsHeader.AppendLine("%%Title: Mock EPS");
            epsHeader.AppendLine("%%Pages: 1");
            epsHeader.AppendLine("%%BoundingBox: 0 0 595 842");
            epsHeader.AppendLine("%%This is a mock EPS file. No real content.");
            epsHeader.AppendLine("showpage");

            var outputStream = new MemoryStream(Encoding.ASCII.GetBytes(epsHeader.ToString()));
            Response.Headers.Add("X-Conversion-Warning", "This is a mock EPS file.");
            return File(outputStream.ToArray(), "application/postscript", "converted.eps");
        }


        [HttpPost("pdf-to-webp")]
        public async Task<IActionResult> PdfToWebp(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Render PDF sang bitmap trước
            using var tempStream = new MemoryStream();
            using (var pdfiumDocument = PdfiumViewer.PdfDocument.Load(memoryStream))
            {
                using var bitmap = pdfiumDocument.Render(0, 300, 300, true);
                bitmap.Save(tempStream, System.Drawing.Imaging.ImageFormat.Png);
            }

            tempStream.Position = 0;

            // Chuyển đổi bitmap sang WebP
            using var image = SixLabors.ImageSharp.Image.Load(tempStream);
            var outputStream = new MemoryStream();

            // Sử dụng WebP encoder với quality 80%
            var encoder = new WebpEncoder { Quality = 80 };
            image.Save(outputStream, encoder);

            outputStream.Position = 0;
            return File(outputStream.ToArray(), "image/webp", "converted.webp");
        }

        [HttpPost("pdf-to-svg")]
        public async Task<IActionResult> PdfToSvg(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var svgContent = new StringBuilder();
            svgContent.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
            svgContent.AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"595\" height=\"842\" viewBox=\"0 0 595 842\">");
            svgContent.AppendLine("<rect width=\"100%\" height=\"100%\" fill=\"white\"/>");
            svgContent.AppendLine("<text x=\"50\" y=\"50\" fill=\"red\">[MOCK] Converted from PDF</text>");
            svgContent.AppendLine("</svg>");

            var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(svgContent.ToString()));
            Response.Headers.Add("X-Conversion-Warning", "This is a mock SVG file. No real PDF content.");
            return File(outputStream.ToArray(), "image/svg+xml", "converted.svg");
        }


        [HttpPost("pdf-to-epub")]
        public async Task<IActionResult> PdfToEpub(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var outputStream = new MemoryStream();
            using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
            {
                // Entry 1: mimetype (must be first, uncompressed)
                var mimetypeEntry = archive.CreateEntry("mimetype", CompressionLevel.NoCompression);
                using (var writer = new StreamWriter(mimetypeEntry.Open()))
                {
                    writer.Write("application/epub+zip");
                }

                // Entry 2: mock notice
                var noticeEntry = archive.CreateEntry("mock_notice.txt");
                using (var noticeWriter = new StreamWriter(noticeEntry.Open()))
                {
                    noticeWriter.WriteLine("This is a placeholder EPUB. No real PDF content included.");
                }
            }

            outputStream.Position = 0;
            Response.Headers.Add("X-Conversion-Warning", "This is a mock EPUB file. Real conversion not implemented.");
            return File(outputStream.ToArray(), "application/epub+zip", "converted.epub");
        }



        [HttpPost("pdf-to-dxf")]
        public async Task<IActionResult> PdfToDxf(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var dxfContent = new StringBuilder();
            dxfContent.AppendLine("0");
            dxfContent.AppendLine("SECTION");
            dxfContent.AppendLine("2");
            dxfContent.AppendLine("HEADER");
            dxfContent.AppendLine("999");
            dxfContent.AppendLine("This is a mock DXF file from PDF.");
            dxfContent.AppendLine("0");
            dxfContent.AppendLine("ENDSEC");
            dxfContent.AppendLine("0");
            dxfContent.AppendLine("EOF");

            var outputStream = new MemoryStream(Encoding.ASCII.GetBytes(dxfContent.ToString()));
            Response.Headers.Add("X-Conversion-Warning", "This is a mock DXF file.");
            return File(outputStream.ToArray(), "application/dxf", "converted.dxf");
        }


        [HttpPost("pdf-to-mobi")]
        public async Task<IActionResult> PdfToMobi(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var mobiHeader = new byte[] { 0x42, 0x4F, 0x4F, 0x4B, 0x4D, 0x4F, 0x42, 0x49 }; // "BOOKMOBI"
            var content = Encoding.UTF8.GetBytes("\n[MOCK FILE] This MOBI is a placeholder. No actual conversion performed.");

            var outputStream = new MemoryStream();
            outputStream.Write(mobiHeader, 0, mobiHeader.Length);
            outputStream.Write(content, 0, content.Length);

            outputStream.Position = 0;
            Response.Headers.Add("X-Conversion-Warning", "This is a mock MOBI file.");
            return File(outputStream.ToArray(), "application/x-mobipocket-ebook", "converted.mobi");
        }


        [HttpPost("pdf-to-azw3")]
        public async Task<IActionResult> PdfToAzw3(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var azwHeader = new byte[] { 0x42, 0x4F, 0x4F, 0x4B, 0x4D, 0x4F, 0x42, 0x49 }; // "BOOKMOBI"
            var content = Encoding.UTF8.GetBytes("\n[MOCK FILE] This AZW3 is a placeholder. No actual content.");

            var outputStream = new MemoryStream();
            outputStream.Write(azwHeader, 0, azwHeader.Length);
            outputStream.Write(content, 0, content.Length);

            outputStream.Position = 0;
            Response.Headers.Add("X-Conversion-Warning", "This is a mock AZW3 file.");
            return File(outputStream.ToArray(), "application/vnd.amazon.ebook", "converted.azw3");
        }


        [HttpPost("pdf-to-image")]
        public async Task<IActionResult> PdfToImage(IFormFile file)
        {
            // Chuyển đổi PDF sang hình ảnh (tương tự PDF to PNG)
            return await PdfToPng(file);
        }
    }
}