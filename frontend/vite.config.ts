import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// ✅ Alias para que @ apunte a /src
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": "/src",
    },
  },
});
