import React, { useState, createContext } from "react";

export const ImageContext = createContext(null)

export const ImageProvider = ({ children }) => {
    const [image, setImage] = useState(null)

    return (
        <ImageContext.Provider value={{image, setImage}}>
            {children}
        </ImageContext.Provider>
    )
}