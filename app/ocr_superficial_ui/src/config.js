const ROOT_API_URL = process.env.REACT_APP_API_URL
const REGISTER_API_EP = process.env.REACT_APP_REGISTER_EP
const LOGIN_API_EP = process.env.REACT_APP_LOGIN_EP
const IS_PRODUCTION = process.env.NODE_ENV == 'production'

const config = {
    rootApiUrl: ROOT_API_URL,
    registerApiEp: REGISTER_API_EP,
    loginApiEp: LOGIN_API_EP,
    isProduction: IS_PRODUCTION
}

export default config