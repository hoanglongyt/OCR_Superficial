import React from "react";

function FileInfo({ file, onConvert, isConverting }) {
  return (
    <div className="file-info">
      <p>Selected file: {file.name}</p>
      <button
        className="convert-button"
        onClick={onConvert}
        disabled={isConverting}
      >
        {isConverting ? "Converting..." : "Convert to Text"}
      </button>
    </div>
  );
}

export default FileInfo;
