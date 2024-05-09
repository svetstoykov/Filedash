import { useState } from "react";

function FileUpload({ onUpload }) {
    const [selectedFiles, setSelectedFiles] = useState([]);

    const handleChange = (event) => {
        setSelectedFiles(() => Array.from(event.target.files));
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        if (selectedFiles.length > 0) {
            onUpload(selectedFiles);
        }
    };

    return (
        <div className="uploader">
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <input
                        type="file"
                        className="form-control"
                        id="file"
                        onChange={handleChange}
                        multiple
                    />
                </div>
                <button type="submit" className="btn btn-primary">
                    Upload
                </button>
            </form>
        </div>
    );
}

export default FileUpload;
