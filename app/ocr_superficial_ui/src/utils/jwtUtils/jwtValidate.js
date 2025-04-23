export function isValidJWT(token) {
    if (!token || typeof token !== "string") {
        return false;
    }

    const parts = token.split('.');
    
    if (parts.length !== 3) {
        console.error("Invalid JWT: Token does not have 3 parts.");
        return false;
    }

    // Validate Base64URL encoding for each part (simple regex check)
    const base64UrlPattern = /^[A-Za-z0-9_-]+$/;
    
    if (
        !base64UrlPattern.test(parts[0]) || // Header
        !base64UrlPattern.test(parts[1]) || // Payload
        !base64UrlPattern.test(parts[2])    // Signature
    ) {
        console.error("Invalid JWT: Base64URL format error.");
        return false;
    }

    console.log("JWT is in the correct format");
    return true;
}