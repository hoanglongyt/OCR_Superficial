import React from 'react';
import { useLocation } from 'react-router-dom';

const PdfConversionLayout = () => {
  const location = useLocation();
  // Extract format from pathname (e.g., /pdf-to-word -> word)
  const format = location.pathname.split('pdf-to-')[1];

  const getTitle = () => {
    switch (format) {
      case 'word': return 'Convert PDF to Word';
      case 'excel': return 'Convert PDF to Excel';
      case 'powerpoint': return 'Convert PDF to PowerPoint';
      case 'html': return 'Convert PDF to HTML';
      case 'text': return 'Convert PDF to Text';
      case 'png': return 'Convert PDF to PNG';
      case 'jpg': return 'Convert PDF to JPG';
      case 'tiff': return 'Convert PDF to TIFF';
      case 'webp': return 'Convert PDF to WebP';
      case 'svg': return 'Convert PDF to SVG';
      case 'epub': return 'Convert PDF to EPUB';
      case 'mobi': return 'Convert PDF to MOBI';
      case 'azw3': return 'Convert PDF to AZW3';
      case 'dxf': return 'Convert PDF to DXF';
      case 'eps': return 'Convert PDF to EPS';
      default: return 'PDF Converter';
    }
  };

  // Add console.log for debugging
  console.log('Current path:', location.pathname);
  console.log('Extracted format:', format);

  return (
    <div className="pdf-conversion-container">
      <div className="container py-5">
        <h1 className="text-center mb-4">{getTitle()}</h1>
        
        <div className="conversion-box p-4 bg-white rounded-3 shadow-sm">
          {/* File Upload Section */}
          <div className="upload-section text-center p-5 border-2 border-dashed rounded-3">
            <input
              type="file"
              accept=".pdf"
              className="d-none"
              id="pdfInput"
            />
            <label htmlFor="pdfInput" className="upload-label">
              <div className="upload-icon mb-3">📄</div>
              <h3 className="h5 mb-2">Choose PDF file or drag & drop it here</h3>
              <p className="text-muted">Maximum file size: 100MB</p>
            </label>
          </div>

          {/* Conversion Options */}
          <div className="options-section mt-4">
            <h4 className="h6 mb-3">Conversion Options</h4>
            {/* Add format-specific options here */}
          </div>

          {/* Convert Button */}
          <div className="action-section mt-4 text-center">
            <button className="btn btn-primary btn-lg px-5">
              Convert to {format?.toUpperCase()}
            </button>
          </div>
        </div>

        {/* Features Section */}
        <div className="features-section mt-5">
          <h2 className="h4 text-center mb-4">Why Convert with Us?</h2>
          <div className="row g-4">
            <div className="col-md-4">
              <div className="feature-card p-3 text-center">
                <div className="feature-icon mb-2">🔒</div>
                <h3 className="h6">Secure Conversion</h3>
                <p className="small text-muted">Your files are automatically deleted after conversion</p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="feature-card p-3 text-center">
                <div className="feature-icon mb-2">⚡</div>
                <h3 className="h6">Fast Processing</h3>
                <p className="small text-muted">Convert your files in seconds</p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="feature-card p-3 text-center">
                <div className="feature-icon mb-2">💎</div>
                <h3 className="h6">High Quality</h3>
                <p className="small text-muted">Maintain original quality and formatting</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PdfConversionLayout;