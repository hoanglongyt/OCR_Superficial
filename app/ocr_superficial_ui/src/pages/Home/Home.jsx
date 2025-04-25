import React, { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ImageContext } from "../../contexts/ImageContext";

import Title from "./Title";
import UploadSection from "./UploadSection";
import FileInfo from "./FileInfo";

import "./Home.css";

function Home() {
  const navigate = useNavigate();
  const { setImage } = useContext(ImageContext);
  const [selectedFile, setSelectedFile] = useState(null);
  const [isConverting, setIsConverting] = useState(false);

  const handleFileChange = (file) => {
    setSelectedFile(file);
  };

  const handleConversion = async () => {
    if (!selectedFile) {
      alert("Please select an Image file first");
      return;
    }

    setIsConverting(true);
    setTimeout(() => {
      setImage(selectedFile);
      navigate("/process");
    }, 1500);
  };

  return (
    <div className="home-container">
      <Title />

      <p className="home-subtagline">
        Upload your receipts, notes, or scanned images. We’ll convert them into text for you ✨
      </p>

      <div className="converter-box">
        <UploadSection onFileSelect={handleFileChange} />
        {selectedFile && (
          <FileInfo
            file={selectedFile}
            onConvert={handleConversion}
            isConverting={isConverting}
          />
        )}
      </div>

      <section className="features-section">
        <h2>Why Use Our Tool?</h2>
        <div className="features-grid">
          <div className="feature-card">🧠 AI-Powered OCR<br /><span>Advanced text detection</span></div>
          <div className="feature-card">📷 Multi-format<br /><span>PNG, JPG, JPEG supported</span></div>
          <div className="feature-card">📄 Export PDF<br /><span>Save as searchable PDFs</span></div>
        </div>
      </section>

      <section className="how-it-works-section">
        <h2>How It Works</h2>
        <div className="steps-grid">
          <div className="step">1️⃣ Upload your image</div>
          <div className="step">2️⃣ We extract the text</div>
          <div className="step">3️⃣ Download your result</div>
        </div>
      </section>

      <section className="demo-section">
        <h2>Live Example</h2>
        <p>Try scanning a sample receipt or photo below</p>
        <div className="demo-placeholder">📸 Demo Screenshot or Animation Here</div>
      </section>

      <section className="cta-section">
        <h2>Ready to get started?</h2>
        <button onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}>Upload Now</button>
      </section>
    </div>
  );
}

export default Home;
