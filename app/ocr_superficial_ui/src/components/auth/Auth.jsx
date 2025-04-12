import React from "react";
import { Route, Routes } from "react-router-dom";
import RegisterForm from "./RegisterForm";
import LoginForm from "./LoginForm";

function Auth() {
    return (
        <div className="container mt-4">
            <div className="row justify-content-center">
                <div className="col-md-6">
                    <Routes>
                        <Route path="/register" element={<RegisterForm />}/>
                        <Route path="/login" element={<LoginForm />}/>
                    </Routes>
                </div>
            </div>
        </div>
    );
}

export default Auth