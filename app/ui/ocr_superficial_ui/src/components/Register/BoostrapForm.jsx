import React from "react";

function BoostrapForm(){
    return (
        <div className="card shadow-sm p-4">
            <h2 className="mb-3 text-center">Simple Form</h2>
            <form action="">
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
                    />
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