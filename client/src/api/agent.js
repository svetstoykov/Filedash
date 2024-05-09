// api.js
import axios from "axios";

const API_BASE_URL = "https://localhost:44338/api";

const instance = axios.create({
    baseURL: API_BASE_URL,
});

const agent = {
    uploadFiles: async (files) => {
        const formData = new FormData();

        files.forEach((file) => {
            formData.append("file", file);
        });

        await instance.post("/files/upload", formData);
    },
    listFiles: async () => {
        const response = await instance.get("/files/list");
        return response.data.data;
    },
    deleteFile: async (id) => {
        await instance.delete(`/files/${id}`);
    },
};

export default agent;
