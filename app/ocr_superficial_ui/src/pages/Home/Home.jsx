import React, { useState } from "react";
import { FiUpload } from "react-icons/fi";
import "./Home.css";

function Home() {
  const [selectedFile, setSelectedFile] = useState(null);
  const [isConverting, setIsConverting] = useState(false);

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file && file.type === "application/pdf") {
      setSelectedFile(file);
    } else {
      alert("Please select a valid PDF file");
    }
  };

  const handleConversion = async () => {
    if (!selectedFile) {
      alert("Please select a PDF file first");
      return;
    }

    setIsConverting(true);
    // TODO: Implement your PDF conversion logic here
    setIsConverting(false);
  };

  return (
    <div className="home-container">
      <h1>PDF to Text Converter</h1>
      <div className="converter-box">
        <div className="upload-section">
          <input
            type="file"
            accept=".pdf"
            onChange={handleFileChange}
            id="file-input"
            hidden
          />
          <label htmlFor="file-input" className="upload-label">
            <FiUpload size={40} />
            <p>Drop your PDF here or click to browse</p>
          </label>
        </div>
        
        {selectedFile && (
          <div className="file-info">
            <p>Selected file: {selectedFile.name}</p>
            <button 
              className="convert-button"
              onClick={handleConversion}
              disabled={isConverting}
            >
              {isConverting ? "Converting..." : "Convert to Text"}
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default Home;