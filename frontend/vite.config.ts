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
    global: "window", 
  },
  optimizeDeps: {
    include: ["jquery", "select2"],
  },
  server: {
    host: true,
    port: 5173
  },
});
