// import reacts
import { useState, useEffect } from "react";
import { Routes, Route } from "react-router-dom";

// import components
import Loading from "./components/Loading";
import Viewing from "./components/Viewing";

// import css
import "./Process.css";

export default function Process() {
    const [image, setImage] = useState(null)

    return (
        <Routes>
            <Route path="/" element={<Loading setImage={setImage} />}/>
            <Route path="/result" element={<Viewing/>}/>
        </Routes>
    )
}
