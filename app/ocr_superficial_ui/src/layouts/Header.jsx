import React, { useState, useRef } from 'react';
import Logo from '../components/logo/Logo';
import Navbar from '../components/header/Navbar';
import { Link } from 'react-router-dom';
import { FaFileWord, FaFileExcel, FaFilePowerpoint, FaFileAlt, FaImage, FaBook, FaVectorSquare } from 'react-icons/fa';
import './Header.css';

const Header = () => {
  const [showDropdown, setShowDropdown] = useState(false);
  const hideTimeoutRef = useRef(null);

  const handleMouseEnter = () => {
    clearTimeout(hideTimeoutRef.current);
    setShowDropdown(true);
  };

  const handleMouseLeave = () => {
    hideTimeoutRef.current = setTimeout(() => {
      setShowDropdown(false);
    }, 200); // Delay 300ms trước khi ẩn
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-white shadow-sm sticky-top">
      <div className="container d-flex justify-content-between align-items-center">
        <div className="d-flex align-items-center">
          <Logo />
          <div
            className="dropdown ms-3 position-relative"
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
          >
            <button className="btn btn-light dropdown-toggle" type="button">
              Convert
            </button>
            {showDropdown && (
              <ul className="dropdown-menu mt-2 shadow p-2 rounded show position-absolute">
                <li className="dropdown-header text-muted">Văn bản</li>
                <li><Link className="dropdown-item" to="/pdf-to-word"><FaFileWord className="me-2 text-primary" />PDF to Word</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-excel"><FaFileExcel className="me-2 text-success" />PDF to Excel</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-powerpoint"><FaFilePowerpoint className="me-2 text-danger" />PDF to PowerPoint</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-html"><FaFileAlt className="me-2 text-warning" />PDF to HTML</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-text"><FaFileAlt className="me-2 text-secondary" />PDF to Text</Link></li>

                <li><hr className="dropdown-divider" /></li>
                <li className="dropdown-header text-muted">Hình ảnh</li>
                <li><Link className="dropdown-item" to="/pdf-to-png"><FaImage className="me-2" />PDF to PNG</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-jpg"><FaImage className="me-2" />PDF to JPG</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-tiff"><FaImage className="me-2" />PDF to TIFF</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-webp"><FaImage className="me-2" />PDF to WebP</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-svg"><FaVectorSquare className="me-2" />PDF to SVG</Link></li>

                <li><hr className="dropdown-divider" /></li>
                <li className="dropdown-header text-muted">Ebook</li>
                <li><Link className="dropdown-item" to="/pdf-to-epub"><FaBook className="me-2" />PDF to EPUB</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-mobi"><FaBook className="me-2" />PDF to MOBI</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-azw3"><FaBook className="me-2" />PDF to AZW3</Link></li>

                <li><hr className="dropdown-divider" /></li>
                <li className="dropdown-header text-muted">Khác</li>
                <li><Link className="dropdown-item" to="/pdf-to-dxf"><FaVectorSquare className="me-2" />PDF to DXF</Link></li>
                <li><Link className="dropdown-item" to="/pdf-to-eps"><FaVectorSquare className="me-2" />PDF to EPS</Link></li>
              </ul>
            )}
          </div>
        </div>

        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <Navbar />
      </div>
    </nav>
  );
};

export default Header;
