import React from "react";
import { Link } from "react-router-dom";

function AuthenOptions(){
    return(
        <div className="ms-lg-3">
            <Link to="/login" className="btn btn-outline-primary rounded-pill me-2 px-3">
              Login
            </Link>
            <Link to="/register" className="btn btn-primary rounded-pill px-3">
              Sign Up
            </Link>
        </div>
    )
}

export default AuthenOptions