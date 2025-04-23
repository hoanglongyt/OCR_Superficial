// import react states and objs
import { useState } from "react";
import { Link } from "react-router-dom";

// import services
import { logoutUser } from "../../services/authService";

function Partial({ user }) {
  const [isDropdownOpen, setDropdownOpen] = useState(false);
  const [isDarkMode, setDarkMode] = useState(false);

  const toggleDropdown = () => {
    setDropdownOpen(!isDropdownOpen);
  };

  const toggleDarkMode = () => {
    setDarkMode(!isDarkMode);
    document.body.classList.toggle("dark-mode", !isDarkMode);
  };

  const handleLogout = () => {
    logoutUser();
  };

  return (
    <div className="d-flex align-items-center gap-2 position-relative">
      {/* Circle Avatar */}
      <div
        onClick={toggleDropdown}
        style={{
          width: "40px",
          height: "40px",
          borderRadius: "50%",
          backgroundColor: "#007bff",
          color: "white",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          fontWeight: "bold",
          fontSize: "1.2rem",
          cursor: "pointer",
          userSelect: "none",
        }}
      >
        {user.unique_name ? user.unique_name.charAt(0).toUpperCase() : "?"}
      </div>

      {/* Dropdown Menu */}
      {isDropdownOpen && (
        <div
          style={{
            position: "absolute",
            top: "50px",
            right: "0",
            backgroundColor: "white",
            boxShadow: "0px 4px 8px rgba(0, 0, 0, 0.1)",
            borderRadius: "8px",
            overflow: "hidden",
            zIndex: 1000,
            width: "200px",
          }}
        >
          <div
            onClick={toggleDarkMode}
            style={{
              padding: "10px",
              cursor: "pointer",
              borderBottom: "1px solid #eee",
            }}
          >
            Theme: {isDarkMode ? "Dark" : "Light"}
          </div>
          <div
            style={{
              padding: "10px",
              cursor: "pointer",
              borderBottom: "1px solid #eee",
            }}
          >
            Language
          </div>
          <Link
            to="/profile"
            style={{
              padding: "10px",
              display: "block",
              cursor: "pointer",
              borderBottom: "1px solid #eee",
              color: "inherit",
              fontStyle: "none"
            }}
          >
            Profile
          </Link>
          <div
            onClick={handleLogout}
            style={{
              padding: "10px",
              cursor: "pointer",
              color: "red",
            }}
          >
            Logout
          </div>
        </div>
      )}
    </div>
  );
}

export default Partial;
