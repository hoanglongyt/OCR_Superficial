import config from "../config"

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

export async function loginUser(user_info) {
    const response = await fetch(`${API_URL}/${AUTH_CONTROLLER}/login`, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user_info)
    })

    return response
}