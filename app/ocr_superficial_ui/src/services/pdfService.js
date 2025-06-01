import config from "../config";

const rootApi = config.rootApiUrl;
const pdfController = "PdfConverter"; // Controller xử lý PDF ở backend

export async function convertPdf(pdfFile, format) {
  const url = `${rootApi}/${pdfController}/convert?format=${format}`;
  const formData = new FormData();
  formData.append("file", pdfFile);

  try {
    const response = await fetch(url, {
      method: "POST",
      body: formData,
    });

    if (!response.ok) {
      throw new Error(`Error: ${response.status}`);
    }

    // Lấy Content-Type từ response header
    const contentType = response.headers.get("content-type");
    const blob = await response.blob();
    
    // Kiểm tra nếu response là file ZIP
    if (contentType === "application/zip") {
      return {
        success: true,
        data: blob,
        filename: `converted_pages.zip`,
        isZip: true
      };
    } else {
      return {
        success: true,
        data: blob,
        filename: `converted.${format}`
      };
    }
  } catch (error) {
    console.error("PDF conversion failed:", error);
    return {
      success: false,
      error: error.message
    };
  }
}

// Các hàm chuyển đổi cụ thể
export function convertPdfToWord(pdfFile) {
  return convertPdf(pdfFile, "docx");
}

export function convertPdfToExcel(pdfFile) {
  return convertPdf(pdfFile, "xlsx");
}

export function convertPdfToPowerPoint(pdfFile) {
  return convertPdf(pdfFile, "pptx");
}

export function convertPdfToHtml(pdfFile) {
  return convertPdf(pdfFile, "html");
}

export function convertPdfToText(pdfFile) {
  return convertPdf(pdfFile, "txt");
}

// Các hàm chuyển đổi sang định dạng ảnh
export function convertPdfToPng(pdfFile) {
  return convertPdf(pdfFile, "png");
}

export function convertPdfToJpg(pdfFile) {
  return convertPdf(pdfFile, "jpg");
}

export function convertPdfToTiff(pdfFile) {
  return convertPdf(pdfFile, "tiff");
}

export function convertPdfToWebp(pdfFile) {
  return convertPdf(pdfFile, "webp");
}

export function convertPdfToSvg(pdfFile) {
  return convertPdf(pdfFile, "svg");
}

// Các hàm chuyển đổi sang định dạng ebook
export function convertPdfToEpub(pdfFile) {
  return convertPdf(pdfFile, "epub");
}

export function convertPdfToMobi(pdfFile) {
  return convertPdf(pdfFile, "mobi");
}

export function convertPdfToAzw3(pdfFile) {
  return convertPdf(pdfFile, "azw3");
}

// Các hàm chuyển đổi khác
export function convertPdfToDxf(pdfFile) {
  return convertPdf(pdfFile, "dxf");
}

export function convertPdfToEps(pdfFile) {
  return convertPdf(pdfFile, "eps");
}






