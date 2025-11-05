import axios from "axios";

// Configura la URL base (de .env o localhost)
const baseURL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5144";

const gymApi = axios.create({
  baseURL: `${baseURL}/api`,
  headers: {
    "Content-Type": "application/json",
  },
  //  IMPORTANTE: el backend no usa cookies, por eso se deja en false
  withCredentials: false,
});

// Interceptor de request: agrega el token JWT automáticamente
gymApi.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      // 🚀 Se asegura que el header Authorization esté definido correctamente
      config.headers.Authorization = `Bearer ${token}`;
    } else {
      // Log opcional para debug
      console.warn("[GymAPI] ⚠️ No se encontró token JWT en localStorage");
    }
    return config;
  },
  (error) => {
    // Captura errores antes de enviar el request
    return Promise.reject(error);
  }
);

// Interceptor de respuesta: captura expiraciones o 401 automáticos
gymApi.interceptors.response.use(
  (response) => response,
  (error) => {
    const url = error.config?.url || "";

    // Ignorar rutas públicas
    const rutasPublicas = [
      "/usuarios/login",
      "/usuarios/register",
      "/socios/registro-publico",
      "/usuarios/forgot-password",
      "/usuarios/reset-password",
    ];

    const esPublica = rutasPublicas.some((r) => url.includes(r));

    if (!esPublica && error.response?.status === 401) {
      console.warn("[GymAPI] ⚠️ Token expirado o no autorizado. Redirigiendo al login...");
      localStorage.removeItem("token");
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);


console.log("[GymAPI] Base URL:", gymApi.defaults.baseURL);

export default gymApi;
