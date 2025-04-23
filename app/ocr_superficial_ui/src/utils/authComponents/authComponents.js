export const handleResult = (message, action) => {
    action(message)
    console.log(message)
}