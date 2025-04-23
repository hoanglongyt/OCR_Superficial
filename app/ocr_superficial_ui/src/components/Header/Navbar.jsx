// import configs
import config from "../../config";

// import reacts states and objects
import { React, useState, useEffect, useContext } from "react";

// import components
import NavbarItems from "./NavbarItems";
import AuthenOptions from "./AuthenOptions";
import Partial from "../users/partial";

// import utils and helper functions
import { isValidJWT } from "../../utils/jwtUtils/jwtValidate";

// import external libraries functions and objects
import { jwtDecode } from 'jwt-decode';
import { UserContext } from "../../contexts/userContext";

function Navbar() {
  const { user } = useContext(UserContext);
  
  return (
    <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
      <NavbarItems />
      
      {user === null && (   // <-- FIXED here
        <AuthenOptions />
      )}

      {user !== null && (
        <Partial user={user} />
      )}
    </div>
  );
}

export default Navbar;
