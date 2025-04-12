// import configs
import config from "../../config";

// import reacts states and objects
import { useNavigate } from "react-router-dom";
import React, { useEffect, useState } from "react";

// import serivces
import { loginUser } from "../../services/authService";

// import helper functions (utils)
import { handleResult } from "../../utils/authComponents/authComponents";

function LoginForm() {
    const navigate = useNavigate()
    const [loginStatusMsg, setLoginStatusMsg] = useState('')
    const [formData, setFormData] = useState({
        username: "",
        password: ""
    });

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
        setLoginStatusMsg('')
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        console.log("Logging in with:", formData);
        const response = await loginUser(formData)
        
        if(response.isSuccess){
            setTimeout(() => {
                navigate('/')
            }, 1000)
        } else {
            setLoginStatusMsg(response.statusMsg)
        }
    };

    useEffect(() => {
        if(!config.isProduction){
            console.log(loginStatusMsg)
            const token = localStorage.getItem("token")
            console.log(token)
        }
    }, [loginStatusMsg])

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Login</h2>
            
            {loginStatusMsg !== '' && (
                <div className="mt-4 alert alert-danger alert-dismissible fade show" role="alert">
                    <strong>Error:</strong> {loginStatusMsg}
                    <button type="button" className="btn-close" data-bs-dismiss="alert" aria-label="Close" onClick={() => handleResult('', setLoginStatusMsg)}></button>
                </div>
            )}

            <form onSubmit={handleSubmit} className="p-4 border rounded shadow-sm">
                <div className="mb-3">
                    <label htmlFor="username" className="form-label">Username:</label>
                    <input
                        type="text"
                        className="form-control"
                        id="username"
                        name="username"
                        value={formData.username}
                        onChange={handleChange}
                        placeholder="Username"
                        required
                    />
                </div>

                <div className="mb-3">
                    <label htmlFor="password" className="form-label">Password:</label>
                    <input
                        type="password"
                        className="form-control"
                        id="password"
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        placeholder="Password"
                        required
                    />
                </div>

                <button type="submit" className="btn btn-primary w-100">Login</button>
            </form>
        </div>
    );
}

export default LoginForm;
