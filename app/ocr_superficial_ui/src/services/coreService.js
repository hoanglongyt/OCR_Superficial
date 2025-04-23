import config from "../config";

const rootApi = config.rootApiUrl
const imageController = config.imageController

const apiCalling = async (image, callback = null, endpoint = "/") => {
    const url = rootApi + "/" + imageController + endpoint
    const payload = new FormData()
    payload.append("imageFile", image)

    return fetch(url, {
        method: "POST",
        body: payload
    })
        .then(async (res) => res.blob())
        .then((blob) => {
            const fileName = endpoint.split("/")[1]
            const file = new File([blob], fileName + ".png", {
                type: blob.type,
                lastModified: Date.now()
            });

            if(callback){
                callback(file);
            }

            return URL.createObjectURL(file)
        })
        .catch((err) => {
            console.log("Uploaded failed: ", err)
            return null;
        })
}

export async function toImage(image, callback = null){
    return apiCalling(image, callback, "/resize-image")
}

export async function greyOut(image, callback = null) {
    return apiCalling(image, callback, "/preprocess-image-for-ocr")
}

export async function extractText(image, callback = null) {
    const url = rootApi + "/" + imageController + "/extract-text-from-image"
    const payload = new FormData()
    payload.append("imageFile", image)

    return fetch(url, {
        method: "POST",
        body: payload
    })
        .then(async (res) => {
            if(callback){
                callback()
            }
            return res.json()
        })
        .catch((err) => {
            console.log("Uploaded failed: ", err)
            return null;
        })
}