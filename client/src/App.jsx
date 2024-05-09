import { useState, useEffect } from "react";
import axios from "axios";
import FileUploader from "./components/FileUploader";
import FilesGrid from "./components/FilesGrid";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./index.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "primereact/resources/themes/soho-light/theme.css";
import "primeicons/primeicons.css";

function App() {
    const [files, setFiles] = useState([]);

    // Fetch files on initial render
    useEffect(() => {
        fetchFiles();
    }, []);

    // Function to fetch files
    const fetchFiles = async () => {
        try {
            const response = await axios.get("https://localhost:44338/api/files/list");
            setFiles(() => [response.data.data]);
        } catch (error) {
            console.error("Error fetching files:", error);
        }
    };

    // Function to handle file upload
    const handleUpload = async (files) => {
        try {
            const formData = new FormData();

            files.forEach((file) => {
                formData.append("file", file);
            });

            await axios.post("https://localhost:44338/api/files/upload", formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            });

            toast.success("File uploaded successfully!");

            fetchFiles();
        } catch (error) {
            error.response.data.forEach((d) => {
                toast.error(`Error uploading file: ${d.message}`);
            });
        }
    };

    // Function to handle file deletion
    const handleDelete = async (id) => {
        try {
            await axios.delete(`https://localhost:44338/api/files/${id}11111`);

            toast.success("File deleted successfully!");

            fetchFiles();
        } catch (error) {
            toast.error(`Error deleting file: ${error}`);
        }
    };

    return (
        <div className="container mt-6">
            <h1>Filedash App</h1>
            <FileUploader onUpload={handleUpload} />
            <FilesGrid files={files} onDelete={handleDelete} />
            <ToastContainer position="top-center" />
        </div>
    );
}

export default App;
