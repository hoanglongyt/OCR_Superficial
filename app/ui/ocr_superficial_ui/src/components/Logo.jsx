import React from "react";
import { Link } from 'react-router-dom'

function Logo(){
    return (
        <Link className="navbar-brand d-flex align-items-center" to="/">
          <img
            src="..\logo.png" // Ensure this path is correct in your public folder
            alt="OCR Superficial Logo"
            height="30"
            className="d-inline-block align-top me-2"
          />
          <span className="fw-semibold">OCR Superficial</span>
        </Link>
    )
}

export default Logo