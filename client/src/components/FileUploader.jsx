import { useState } from "react";
import { ProgressSpinner } from "primereact/progressspinner";

function FileUpload({ onUpload }) {
    const [selectedFiles, setSelectedFiles] = useState([]);
    const [inputKey, setinputKey] = useState(null);
    const [showLoader, setShowLoader] = useState(false);

    const handleChange = (event) => {
        setSelectedFiles(() => Array.from(event.target.files));
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        if (selectedFiles.length > 0) {
            setShowLoader(() => true);

            await onUpload(selectedFiles);

            setShowLoader(() => false);

            // rerender the input so that the files are cleared
            setinputKey(() => Date.now());
        }
    };

    return (
        <div className="uploader">
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <input
                        key={inputKey}
                        type="file"
                        className="form-control"
                        id="file"
                        onChange={handleChange}
                        multiple
                        accept=".*"
                        data-max-size="1073741824"
                        title="Maximum file size: 1GB"
                    />
                </div>
                <button
                    type="submit"
                    className="btn btn-primary"
                    disabled={selectedFiles && selectedFiles.length ? false : true}
                >
                    Upload
                </button>
            </form>
            <ProgressSpinner
                style={{
                    width: "50px",
                    height: "50px",
                    visibility: showLoader ? "visible" : "hidden",
                }}
                strokeWidth="8"
                fill="var(--surface-ground)"
                animationDuration=".5s"
            />
        </div>
    );
}

export default FileUpload;
