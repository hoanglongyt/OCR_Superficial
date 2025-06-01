const ROOT_API_URL = import.meta.env.VITE_API_URL
const AUTH_CONTROLLER = import.meta.env.VITE_AUTH_CONTROLLER
const IMAGE_CONTROLLER = import.meta.env.VITE_IMAGE_CONTROLLER
const PDF_CONTROLLER = import.meta.env.VITE_PDF_CONTROLLER || "PdfConverter"
const IS_PRODUCTION = import.meta.env.NODE_ENV == 'production'

const config = {
    rootApiUrl: ROOT_API_URL,
    authController: AUTH_CONTROLLER,
    imageController: IMAGE_CONTROLLER,
    pdfController: PDF_CONTROLLER,
    isProduction: IS_PRODUCTION
}

export default config
