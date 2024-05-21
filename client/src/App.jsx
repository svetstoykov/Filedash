import { useState, useEffect } from "react";
import FileUploader from "./components/FileUploader";
import FilesGrid from "./components/FilesGrid";
import agent from "./api/agent";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./index.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "primereact/resources/themes/soho-light/theme.css";
import "primeicons/primeicons.css";

function App() {
    const [files, setFiles] = useState([]);

    useEffect(() => {
        fetchFiles();
    }, []);

    const fetchFiles = async () => {
        try {
            const filesList = await agent.listFiles();

            setFiles(() => [filesList]);
        } catch (error) {
            toast.error(error.response.data.message);
        }
    };

    const handleUpload = async (files) => {
        try {
            await agent.uploadFiles(files);

            toast.success("File uploaded successfully!");

            fetchFiles();
        } catch (error) {
            error.response.data.forEach((d) => {
                toast.error(d.message);
            });
        }
    };

    const handleDelete = async (id) => {
        try {
            await agent.deleteFile(id);

            toast.success("File deleted successfully!");

            fetchFiles();
        } catch (error) {
            toast.error(error.response.data.message);
        }
    };

    const handleDownload = async (id) => {
        try {
            const response = await agent.downloadFile(id);

            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement("a");
            link.href = url;

            var filename = "";
            const disposition = response.headers['content-disposition'];
            if (disposition && disposition.indexOf('attachment') !== -1) {
                const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                const matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }

            link.setAttribute("download", filename);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            toast.error(error.response.data.message);
        }
    };

    return (
        <div className="container mt-6">
            <h1>Filedash App</h1>
            <FileUploader onUpload={handleUpload} />
            <FilesGrid files={files} onDelete={handleDelete} onDownload={handleDownload} />
            <ToastContainer position="top-center" />
        </div>
    );
}

export default App;
