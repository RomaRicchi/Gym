import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": "/src", 
    },
  },
  define: {
    global: "window", // 👈 necesario para jQuery / Select2
  },
  optimizeDeps: {
    include: ["jquery", "select2"], // 👈 asegura que Vite los procese correctamente
  },
});
