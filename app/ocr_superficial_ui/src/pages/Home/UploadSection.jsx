import React, { useState } from "react";
import { FiUpload } from "react-icons/fi";
import config from "../../config";

function UploadSection({ onFileSelect }) {
  const [isDragging, setIsDragging] = useState(false);

  const handleDragOver = (event) => {
    event.preventDefault();
    event.stopPropagation();
    setIsDragging(true);
  };

  const handleDragLeave = (event) => {
    event.preventDefault();
    event.stopPropagation();
    setIsDragging(false);
  };

  const handleDrop = (event) => {
    event.preventDefault();
    event.stopPropagation();
    setIsDragging(false);

    const file = event.dataTransfer.files[0];
    if (!file) return;

    if (!config.isProduction) {
      console.log("Dropped file type: ", file.type);
    }

    if (file.type.startsWith("image/")) {
      onFileSelect(file);
    } else {
      alert("Please drop a valid Image file");
    }
  };

  const handleChange = (event) => {
    const file = event.target.files[0];
    if (!file) return;

    if (!config.isProduction) {
      console.log("Selected file type: ", file.type);
    }

    if (file.type.startsWith("image/")) {
      onFileSelect(file);
    } else {
      alert("Please select a valid Image file");
    }
  };

  return (
    <div
      className={`upload-section ${isDragging ? "dragging" : ""}`}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
    >
      <input
        type="file"
        accept=".png,.jpg,.jpeg"
        id="file-input"
        hidden
        onChange={handleChange}
      />
      <label htmlFor="file-input" className="upload-label">
        <FiUpload size={40} />
        <p>Drag & Drop your PNG here or Click to Browse</p>
      </label>
    </div>
  );
}

export default UploadSection;
