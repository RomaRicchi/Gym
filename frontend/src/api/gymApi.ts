import axios from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5144";

const gymApi = axios.create({
  baseURL: `${baseURL}/api`,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: false,
});

gymApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

console.log("[GymAPI] Base URL:", gymApi.defaults.baseURL);

export default gymApi; 
