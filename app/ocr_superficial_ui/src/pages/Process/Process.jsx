// import reacts
import { useState, useEffect } from "react";
import { Routes, Route } from "react-router-dom";

// import components
import Loading from "../../components/process/Loading";
import Viewing from "../../components/process/Viewing";

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
