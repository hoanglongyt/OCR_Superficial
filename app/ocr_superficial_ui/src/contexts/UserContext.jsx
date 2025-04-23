import React, { useState, createContext } from "react";

export const UserContext = createContext(null)

export const UserProvider = ({ children }) => {
    const [user, setUser] = useState(() => {
        const savedUserToken = localStorage.getItem("token");
        return savedUserToken ? savedUserToken : null
    })

    return (
        <UserContext.Provider value={{user, setUser}}>
            {children}
        </UserContext.Provider>
    )
}