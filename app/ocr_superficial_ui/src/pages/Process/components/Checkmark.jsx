// src/pages/Loading/Checkmark.jsx
import React from "react";

export default function Checkmark() {
  return (
    <div className="checkmark-wrapper">
      <div className="checkmark-circle">
        <svg
          viewBox="0 0 52 52"
          className="checkmark"
          xmlns="http://www.w3.org/2000/svg"
        >
          <circle className="checkmark-circle-bg" cx="26" cy="26" r="25" fill="none" />
          <path
            className="checkmark-check"
            fill="none"
            d="M14 27l7 7 16-16"
          />
        </svg>
      </div>
      <p>Processing complete! Redirecting...</p>
    </div>
  );
}
