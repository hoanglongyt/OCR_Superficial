import React from "react";
import { Route, Routes } from "react-router-dom";
import RegisterForm from "./RegisterForm";
import LoginForm from "./LoginForm";
import { useContext } from "react";
import { UserContext } from "../../contexts/userContext";

function Auth() {
    const {setUser} = useContext(UserContext);

    return (
        <div className="container mt-4">
            <div className="row justify-content-center">
                <div className="col-md-6">
                    <Routes>
                        <Route path="/register" element={<RegisterForm setUser={setUser} />}/>
                        <Route path="/login" element={<LoginForm setUser={setUser} />}/>
                    </Routes>
                </div>
            </div>
        </div>
    );
}

export default Auth