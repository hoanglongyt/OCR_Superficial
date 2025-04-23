import config from "../config";

const rootApi = config.rootApiUrl
const imageController = config.imageController

export async function toImage(image, callback = null){
    const url = rootApi + "/" + imageController + "/resize-image"
    const payload = new FormData()
    payload.append("imageFile", image)

    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type" : image.type,
            // add credentials on needs
        }, 
        body: payload
    })
        .then(async (res) => {
            const response = await res.blob()
            if(callback){
                callback()
            }
            return URL.createObjectURL(response)
        })
        .catch((err) => console.log("Uploaded failed: ", err))
}