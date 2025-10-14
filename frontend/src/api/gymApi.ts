import axios from "axios";

/**
 * Cliente Axios configurado para tu API del gimnasio.
 * Cambiá el puerto según el que uses en tu backend .NET.
 */
const gymApi = axios.create({
  baseURL: "http://localhost:5100/api", // 👈 ajustá si tu API usa otro puerto
  headers: {
    "Content-Type": "application/json",
  },
});

/**
 * Interceptor de solicitudes (request)
 * — agrega token JWT si está guardado
 */
gymApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

/**
 * Interceptor de respuestas (response)
 * — permite capturar errores globales (401, 500, etc.)
 */
gymApi.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.warn("⚠️ Sesión expirada o no autorizada");
      // opcional: redirigir al login
      // window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default gymApi;
