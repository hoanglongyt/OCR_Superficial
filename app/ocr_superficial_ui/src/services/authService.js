import config from "../config"

import { isValidJWT } from "../utils/jwtUtils/jwtValidate"

const API_URL = config.rootApiUrl
const AUTH_CONTROLLER = config.authController

export async function registerUser(new_user) {
    const response = await fetch(`${API_URL}/${AUTH_CONTROLLER}/register`, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(new_user)
    })

    return response
}

export async function loginUser(user_info, fake_login = false) {
    if(fake_login && !config.isProduction){
        const response_obj = {
            isSuccess: true,
            statusMsg: "Successsfully logged in"
        }

        localStorage.setItem("token", "abcdefhgujttkad");
        return response_obj;
    }

    const response_obj = {
        isSuccess: false,
        statusMsg: ''
    }

    const response = await fetch(`${API_URL}/${AUTH_CONTROLLER}/login`, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user_info)
    })

    if(response.ok){
        const responseData = await response.json()
        if(!(responseData.token != null && isValidJWT(responseData.token)))
        {
            console.log("Unable to authorzie users due to the lost of jwt tokens. Contact backend for more information and fixes.")
            // handle calls to backend to say something has been wrong
            return
        }

        localStorage.setItem("token", responseData.token)
        response_obj.isSuccess = true
        response_obj.statusMsg = "Successfully logged in user"
    } else {
        let statusMsg = ''
        switch (response.status){
            case 401: {
                statusMsg = "Cannot authorize user, please check username and password."
                break
            }
        }
        
        response_obj.isSuccess = false
        response_obj.statusMsg = statusMsg
    }

    return response_obj
}

export function logoutUser(){
    localStorage.removeItem("token")
    window.location.reload();
    // expose token from backend
}