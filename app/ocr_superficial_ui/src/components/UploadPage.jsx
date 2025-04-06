import React, { useState } from "react";

function UploadPage() {
    const [file, setFile] = useState(null)
    const [uploadStatus, setUploadStatus] = useState(false)

    const handleFileChane = (event) => {
        setFile(event.target.files[0])
        setUploadStatus(true)
        console.log("Received my!")
    }

    const handleUploadFile = () => {
        console.log("Uploading file...")
    }

    return (
        <div>
            <input type="file" onChange={handleFileChane} />
            {file && (
                <div>
                    <p>Selected file: {file.name}</p>
                    <button onClick={handleUploadFile}>Upload Files</button>
                </div>
            )}
        </div>
    )
}

export default UploadPage