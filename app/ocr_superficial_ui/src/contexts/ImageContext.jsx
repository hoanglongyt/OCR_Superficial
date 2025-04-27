import React, { useState, createContext, useEffect } from "react";

import { deleteImage, saveImage } from "../services/dbService";

import config from "../config";

export const ImageContext = createContext(null)

export const ImageProvider = ({ children }) => {
    const [image, setImage] = useState(null)

    useEffect(() => {
        if(!config.isProduction){
            console.log("File has been changed.")
            console.log(image)
        }
        saveImage("processImage", image)
        if(!image){
            deleteImage("processImage")
        }
    }, [image])

    return (
        <ImageContext.Provider value={{image, setImage}}>
            {children}
        </ImageContext.Provider>
    )
}