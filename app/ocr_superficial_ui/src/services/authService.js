export async function registerUser(url, new_user) {
    const response = await fetch(url, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(new_user)
    })

    console.log(response)

    return response
}