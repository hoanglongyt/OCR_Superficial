import { useState, useEffect } from "react";
import { useLocation } from "react-router-dom";
import "./PdfConversionLayout.css";
import {
  convertPdfToWord,
  convertPdfToExcel,
  convertPdfToPowerPoint,
  convertPdfToHtml,
  convertPdfToText,
  convertPdfToPng,
  convertPdfToJpg,
  convertPdfToTiff,
  convertPdfToWebp,
  convertPdfToSvg,
  convertPdfToEpub,
  convertPdfToMobi,
  convertPdfToAzw3,
  convertPdfToDxf,
  convertPdfToEps
} from "../../services/pdfService";

function PdfConversionLayout() {
  const [file, setFile] = useState(null);
  const [isConverting, setIsConverting] = useState(false);
  const [error, setError] = useState("");
  const [fileName, setFileName] = useState("");
  const [isDragging, setIsDragging] = useState(false);
  const location = useLocation();

  // Reset file khi chuyển đổi giữa các trang
  useEffect(() => {
    // Reset trạng thái khi đường dẫn thay đổi
    setFile(null);
    setFileName("");
    setError("");
  }, [location.pathname]);

  // Xác định loại chuyển đổi dựa trên đường dẫn
  const getConversionType = () => {
    const path = location.pathname;
    // Document formats
    if (path.includes("pdf-to-word")) return { type: "Word", func: convertPdfToWord, ext: "docx" };
    if (path.includes("pdf-to-excel")) return { type: "Excel", func: convertPdfToExcel, ext: "xlsx" };
    if (path.includes("pdf-to-powerpoint")) return { type: "PowerPoint", func: convertPdfToPowerPoint, ext: "pptx" };
    if (path.includes("pdf-to-html")) return { type: "HTML", func: convertPdfToHtml, ext: "html" };
    if (path.includes("pdf-to-text")) return { type: "Text", func: convertPdfToText, ext: "txt" };

    // Image formats
    if (path.includes("pdf-to-png")) return { type: "PNG", func: convertPdfToPng, ext: "png" };
    if (path.includes("pdf-to-jpg")) return { type: "JPG", func: convertPdfToJpg, ext: "jpg" };
    if (path.includes("pdf-to-tiff")) return { type: "TIFF", func: convertPdfToTiff, ext: "tiff" };
    if (path.includes("pdf-to-webp")) return { type: "WebP", func: convertPdfToWebp, ext: "webp" };
    if (path.includes("pdf-to-svg")) return { type: "SVG", func: convertPdfToSvg, ext: "svg" };

    // Ebook formats
    if (path.includes("pdf-to-epub")) return { type: "EPUB", func: convertPdfToEpub, ext: "epub" };
    if (path.includes("pdf-to-mobi")) return { type: "MOBI", func: convertPdfToMobi, ext: "mobi" };
    if (path.includes("pdf-to-azw3")) return { type: "AZW3", func: convertPdfToAzw3, ext: "azw3" };

    // Other formats
    if (path.includes("pdf-to-dxf")) return { type: "DXF", func: convertPdfToDxf, ext: "dxf" };
    if (path.includes("pdf-to-eps")) return { type: "EPS", func: convertPdfToEps, ext: "eps" };

    return { type: "Unknown", func: null, ext: "" };
  };

  const conversionInfo = getConversionType();

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    console.log("Selected file:", selectedFile); // Debug log

    if (selectedFile && selectedFile.type === "application/pdf") {
      setFile(selectedFile);
      setFileName(selectedFile.name);
      setError("");
    } else {
      setFile(null);
      setFileName("");
      setError("Vui lòng chọn file PDF");
    }
  };

  // Xử lý kéo thả file
  const handleDragOver = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(true);
  };

  const handleDragLeave = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  };

  const handleDrop = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);

    const droppedFile = e.dataTransfer.files[0];
    console.log("Dropped file:", droppedFile); // Debug log

    if (droppedFile && droppedFile.type === "application/pdf") {
      setFile(droppedFile);
      setFileName(droppedFile.name);
      setError("");
    } else {
      setFile(null);
      setFileName("");
      setError("Vui lòng chọn file PDF");
    }
  };

  const handleConvert = async () => {
    if (!file) {
      setError("Vui lòng chọn file PDF");
      return;
    }

    setIsConverting(true);
    setError("");

    try {
      console.log("Converting file:", file); // Debug log
      const result = await conversionInfo.func(file);

      if (result.success) {
        // Tạo link tải xuống
        const url = URL.createObjectURL(result.data);
        const a = document.createElement("a");
        a.href = url;

        // Sử dụng tên file từ response, xử lý trường hợp file ZIP
        if (result.isZip) {
          a.download = `${fileName.replace('.pdf', '')}_pages.zip`;
        } else {
          a.download = `${fileName.replace('.pdf', '')}.${conversionInfo.ext}`;
        }

        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
      } else {
        setError(`Lỗi chuyển đổi: ${result.error}`);
      }
    } catch (err) {
      console.error("Conversion error:", err); // Debug log
      setError(`Đã xảy ra lỗi: ${err.message}`);
    } finally {
      setIsConverting(false);
    }
  };

  return (
    <div className="pdf-container">
      <h1>Chuyển đổi PDF sang {conversionInfo.type}</h1>

      <p className="pdf-subtagline">
        Chuyển đổi file PDF sang định dạng {conversionInfo.type} chỉ với vài bước đơn giản ✨
      </p>

      <div className="converter-box">
        <div
          className={`upload-section ${isDragging ? "dragging" : ""}`}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
        >
          <input
            type="file"
            id="pdfFileInput"
            accept=".pdf"
            onChange={handleFileChange}
            hidden
          />
          <label
            htmlFor="pdfFileInput"
            className="upload-label"
          >
            <div style={{ fontSize: '3rem' }}>📄</div>
            <div>Chọn file PDF hoặc kéo thả vào đây</div>
            <p style={{ fontSize: '0.9rem', opacity: 0.8 }}>Kích thước tối đa: 100MB</p>
          </label>
        </div>

        {fileName && (
          <div className="file-info">
            <p>File đã chọn: <strong>{fileName}</strong></p>

            {conversionInfo.type === "PNG" && (
              <div style={{ marginTop: '10px', fontSize: '0.9rem', color: '#666' }}>
                <p>Lưu ý: Khi chuyển đổi PDF sang PNG, bạn sẽ nhận được một file ZIP chứa tất cả các trang của PDF dưới dạng ảnh PNG.</p>
              </div>
            )}

            <button
              onClick={handleConvert}
              disabled={isConverting}
              className="convert-button"
            >
              {isConverting ? "Đang chuyển đổi..." : `Chuyển đổi sang ${conversionInfo.type}`}
            </button>
          </div>
        )}
      </div>

      {error && <div style={{ color: '#ef4444', marginTop: '1rem', textAlign: 'center' }}>{error}</div>}

      <section className="features-section">
        <h2>Tại sao sử dụng công cụ của chúng tôi?</h2>
        <div className="features-grid">
          <div className="feature-card">🚀 Chuyển đổi nhanh chóng<br /><span>Không cần đăng ký</span></div>
          <div className="feature-card">🔒 Bảo mật<br /><span>File của bạn được xử lý an toàn</span></div>
          <div className="feature-card">💯 Chất lượng cao<br /><span>Giữ nguyên định dạng gốc</span></div>
        </div>
      </section>

      <section className="how-it-works-section">
        <h2>Cách thức hoạt động</h2>
        <div className="steps-grid">
          <div className="step">1️⃣ Tải lên file PDF của bạn</div>
          <div className="step">2️⃣ Chúng tôi chuyển đổi file</div>
          <div className="step">3️⃣ Tải xuống kết quả</div>
        </div>
      </section>
    </div>
  );
}

export default PdfConversionLayout;
