// api.js
import axios from "axios";

const instance = axios.create({
    baseURL: import.meta.env.VITE_FILEDASH_API_URL,
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
