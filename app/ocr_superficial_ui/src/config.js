const ROOT_API_URL = import.meta.env.VITE_API_URL
const AUTH_CONTROLLER = import.meta.env.VITE_AUTH_CONTROLLER
const IS_PRODUCTION = import.meta.env.NODE_ENV == 'production'

const config = {
    rootApiUrl: ROOT_API_URL,
    authController: AUTH_CONTROLLER,
    isProduction: IS_PRODUCTION
}

export default config