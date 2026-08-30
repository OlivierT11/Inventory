import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
    plugins: [react()],
    server: {
        watch: {
            // Ignore Visual Studio workspace files (and other common noisy folders)
            ignored: ["**/.vs/**", "**/node_modules/**", "**/.git/**"],
        },
    },
});