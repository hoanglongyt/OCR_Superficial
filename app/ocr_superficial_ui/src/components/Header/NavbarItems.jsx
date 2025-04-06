import React from "react";
import { Link } from "react-router-dom";

function NavbarItems(){
    return (
        <ul className="navbar-nav me-auto me-lg-0">
            <li className="nav-item">
              <Link className="nav-link" to="/">
                Home
              </Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/about">
                About
              </Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/services">
                Services
              </Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/contact">
                Contact
              </Link>
            </li>
            {/* Add more navigation links as needed */}
          </ul>
    )
}

export default NavbarItems