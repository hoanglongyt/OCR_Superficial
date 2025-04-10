// import js services
import { registerUser } from "../../services/authService";

// import react components
import { useNavigate } from "react-router-dom";
import React, { useEffect, useState } from "react";

// import styles
import '../../styles/components/Register/BoostrapForm.css';

function BoostrapForm(){
    const navigate = useNavigate()
    const [isPassValid, setIsPassValid] = useState(false)
    const [userState, setUserState] = useState({
        username: '',
        password: '',
        confirm_password: '',
        email: ''
    })
    const [isRegSuccess, setIsRegSuccess] = useState(false)
    const [isRegFailed, setIsRegFailed] = useState(false)

    // Thay đổi khi nhập thông tin người dùng đăng ký
    const onChangeUserState = (e) => {
        setUserState({
            ...userState,
            [e.target.name]: e.target.value
        })
    }

    // Kiểm tra password khi nhập mật khẩu và password và confirm password
    const validPassword = () => {
        const password = document.getElementById("password").value
        const confirmationPassword = document.getElementById("confirm-password").value
        
        setIsPassValid(password == confirmationPassword)
    }

    const tryCatchingHandleSubmit = () => {
        try{
            handleSubmit()
        } catch (e) {
            localStorage.setItem('lastError', JSON.stringify(e))
        }
    }

    // Xử lý đăng ký
    const handleSubmit = async () => {
        // Xử lý kết quả trả về
        const handleResult = (response, element_name, callback) => {
            const responseData = response.body.json()
            const responseMessage = responseData.message
            document.querySelector(`.${element_name}`).innerHTML = responseMessage
            console.log(responseMessage)
            
            callback(true)
        }

        const new_user = userState
        const response = await registerUser("https://localhost:7268/api/Auth/register", new_user) // Gọi đăng ký

        // Nếu thành công hoặc không
        if(response.ok){
            handleResult(response, "register-success", setIsRegSuccess)
            // handle login automatically
            setTimeout(() => {
                navigate('/')
            }, 1000)

        } else {
            handleResult(response, "register-failed", setIsRegFailed)
        }
    }

    useEffect(() => {
        console.log(userState)
        console.log("is password matched? " + isPassValid)
        const lastError = JSON.parse(localStorage.getItem('lastError'))
        console.log('Last error is: ' + lastError)
        validPassword()
    }, [userState])

    return (
        <div className="card shadow-sm p-4">
            <h2 className="mb-3 text-center">Register Form</h2>
            {isRegSuccess && !isRegFailed && (
                <div className="text-center mt-4">
                    <h2 className="register-success text-success"></h2>
                </div>
            )}
            {isRegFailed && (
                <div className="text-center mt-4">
                    <h2 className="register-success text-failed"></h2>
                </div>
            )}
            <form onSubmit={tryCatchingHandleSubmit}>
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
                    />
                </div>
                <div className="position-relative mb-3">
                    <label htmlFor="confirm-password" className="form-label">
                        Confirm Password:
                    </label>
                    <input 
                        type="password"
                        className="form-control"
                        name="confirm-password"
                        id="confirm-password"
                        placeholder="Confirm your password"
                        onChange={onChangeUserState}
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
                    />
                </div>
                <button type="submit" className="btn btn-primary w-100">
                    Submit
                </button>
            </form>
        </div>
    )
}

export default BoostrapForm