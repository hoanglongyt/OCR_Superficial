// import configs
import config from "../../config";

// import reacts states and objects
import { React, useState, useEffect } from "react";

// import components
import NavbarItems from "./NavbarItems";
import AuthenOptions from "./AuthenOptions";
import Partial from "../users/partial";

// import utils and helper functions
import { isValidJWT } from "../../utils/jwtUtils/jwtValidate";

// import external libraries functions and objects
import { jwtDecode } from 'jwt-decode';

function Navbar() {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const user_token = localStorage.getItem("token");
    console.log("From nav bar: " + user_token)
    if (isValidJWT(user_token)) {
      const decodedUser = jwtDecode(user_token);
      setUser(decodedUser);
    }
  }, []); // Runs once when Navbar mounts

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
