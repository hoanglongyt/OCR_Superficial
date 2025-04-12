// import config
import config from "../../config";

// import js services
import { loginUser, registerUser } from "../../services/authService";

// import react components
import { useNavigate } from "react-router-dom";
import React, { useEffect, useState } from "react";

// import styles
import '../../styles/components/Register/BootstrapForm.css';

import { handleResult } from "../../utils/authComponents/authComponents";

function RegisterForm(){
    const navigate = useNavigate()
    const [onSuccessMessage, setOnSuccessMessage] = useState('')
    const [onFailedMessage, setOnFailedMessage] = useState('')
    const [isPassValid, setIsPassValid] = useState(false)
    const [userState, setUserState] = useState({
        username: '',
        password: '',
        confirm_password: '',
        email: ''
    })

    // Thay đổi khi nhập thông tin người dùng đăng ký
    const onChangeUserState = (e) => {
        setUserState({
            ...userState,
            [e.target.name]: e.target.value
        })
        setOnFailedMessage('')
        setOnSuccessMessage('')
    }

    // Kiểm tra password khi nhập mật khẩu và password và confirm password
    const validPassword = () => {
        const password = document.getElementById("password").value
        const confirmationPassword = document.getElementById("confirm-password").value
        
        setIsPassValid(password == confirmationPassword)
    }

    const tryCatchingHandleSubmit = (event) => {
        try{
            handleSubmit(event)
        } catch (e) {
            localStorage.setItem('lastError', JSON.stringify(e))
        }
    }

    // Xử lý đăng ký
    const handleSubmit = async (event) => {
        event.preventDefault()
        const new_user = userState
        const response = await registerUser(new_user) // Gọi đăng ký

        // Nếu thành công hoặc không
        if(response.ok){
            const responseData = await response.json()
            const message = responseData.message;
            handleResult(message, setOnSuccessMessage)
            
            loginUser({
                username: new_user.username,
                password: new_user.password
            })

            setTimeout(() => {
                navigate('/')
            }, 2000)

        } else {
            const responseData = await response.json()
            const message = responseData[0].description

            handleResult(message, setOnFailedMessage)
        }
    }

    useEffect(() => {
        if(!config.isProduction){
            console.log(userState)
            console.log("is password matched? " + isPassValid)
            const lastError = JSON.parse(localStorage.getItem('lastError'))
            console.log('Last error is: ' + lastError)
        }

        validPassword()
    }, [userState])

    return (
        <div className="container mt-5">
            <h2 className="mb-3 text-center">Register Form</h2>
            {onSuccessMessage !== '' && (
                <div className="mt-4 alert alert-success alert-dismissible fade show" role="alert">
                    <strong>Success:</strong> {onSuccessMessage}
                    <button type="button" className="btn-close" data-bs-dismiss="alert" aria-label="Close" onClick={() => handleResult('', setOnSuccessMessage)}></button>
                </div>
            )}

            {onFailedMessage !== '' && (
                <div className="mt-4 alert alert-danger alert-dismissible fade show" role="alert">
                    <strong>Error:</strong> {onFailedMessage}
                    <button type="button" className="btn-close" data-bs-dismiss="alert" aria-label="Close" onClick={() => handleResult('', setOnFailedMessage)}></button>
                </div>
            )}

            <form onSubmit={tryCatchingHandleSubmit} className="p-4 border rounded shadow-sm">
                <div className="mb-3">
                    <label htmlFor="username" className="form-label">
                        Username:
                    </label>
                    <input
                        type="text"
                        className="form-control"
                        name="username"
                        id="username"
                        placeholder="Username"
                        onChange={onChangeUserState}
                        required
                    />
                </div>
                <div className="mb-3">
                    <label htmlFor="password" className="form-label">
                        Password:
                    </label>
                    <input
                        type="password"
                        className="form-control"
                        name="password"
                        id="password"
                        placeholder="Password"
                        onChange={onChangeUserState}
                        required
                    />
                </div>
                <div className="position-relative mb-3">
                    <label htmlFor="confirm_password" className="form-label">
                        Confirm Password:
                    </label>
                    <input 
                        type="password"
                        className="form-control"
                        name="confirm_password"
                        id="confirm-password"
                        placeholder="Confirm your password"
                        onChange={onChangeUserState}
                        required
                    />

                    {!isPassValid && (
                        <div className="password-popup">
                            <small>
                                Confirmation password must match the Password
                            </small>
                        </div>
                    )}

                </div>
                <div className="mb-3">
                    <label htmlFor="email" className="form-label">
                        Email:
                    </label>
                    <input
                        type="email"
                        className="form-control"
                        name="email"
                        id="email"
                        placeholder="Email"
                        onChange={onChangeUserState}
                        required
                    />
                </div>
                <button type="submit" className="btn btn-primary w-100">
                    Submit
                </button>
            </form>
        </div>
    )
}

export default RegisterForm