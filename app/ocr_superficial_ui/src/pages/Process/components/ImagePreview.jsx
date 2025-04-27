// src/pages/Loading/ImagePreview.jsx
import React from "react";

export default function ImagePreview({ imagePath, imageLoaded, setImageLoaded }) {
  return (
    <div
      className="image-preview-container"
      style={{
        width: "200px",
        height: "200px",
        marginTop: "20px",
        borderRadius: "8px",
        overflow: "hidden",
        backgroundColor: "#f0f0f0",
        position: "relative",
      }}
    >
      {!imageLoaded && (
        <div className="loading-bone" />
      )}

      {imagePath && (
        <img
          src={imagePath}
          alt="Processed"
          onLoad={() => setImageLoaded(true)}
          style={{
            width: "100%",
            height: "100%",
            objectFit: "cover",
            display: imageLoaded ? "block" : "none",
          }}
        />
      )}
    </div>
  );
}
