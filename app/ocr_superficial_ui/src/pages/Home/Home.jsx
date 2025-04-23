import React, { useContext, useState } from "react";
import { FiUpload } from "react-icons/fi";
import "./Home.css";
import config from "../../config";
import { useNavigate } from "react-router-dom";
import { ImageContext } from "../../contexts/ImageContext";

function Home() {
  const navigate = useNavigate()
  const {setImage} = useContext(ImageContext)
  const [selectedFile, setSelectedFile] = useState(null);
  const [isConverting, setIsConverting] = useState(false);

  const handleFileChange = (event) => {
    const file = event.target.files[0];

    if(!config.isProduction){
      console.log("File type: ", file.type);
    }

    if (file && file.type.startsWith("image/")) {
      setSelectedFile(file);
    } else {
      alert("Please select a valid Image file");
    }
  };

  const handleConversion = async () => {
    if (!selectedFile) {
      alert("Please select a Image file first");
      return;
    }

    setIsConverting(true);
    // TODO: Implement your PDF conversion logic here
    setTimeout(() => {
      setImage(selectedFile)
      navigate("/process")
    }, 1500);
  };

  return (
    <div className="home-container">
      <h1>Image to PDF Converter</h1>
      <div className="converter-box">
        <div className="upload-section">
          <input
            type="file"
            accept=".png,.jpg,.jpeg"
            onChange={handleFileChange}
            id="file-input"
            hidden
          />
          <label htmlFor="file-input" className="upload-label">
            <FiUpload size={40} />
            <p>Drop your PNG here or click to browse</p>
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