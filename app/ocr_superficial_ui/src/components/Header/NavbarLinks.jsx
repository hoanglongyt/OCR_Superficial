import React from "react";
import NavbarItems from "./NavbarItems";
import AuthenOptions from "./AuthenOptions";

function NavbarLinks(){
    return (
        <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
          <NavbarItems />
          <AuthenOptions />
        </div>
    )
}

export default NavbarLinks