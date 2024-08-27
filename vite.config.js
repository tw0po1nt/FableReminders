import path from "node:path";
import { fileURLToPath } from "node:url";
import { defineConfig } from "vite";
import fable from "vite-plugin-fable";

const currentDir = path.dirname(fileURLToPath(import.meta.url));
const fsproj = path.join(currentDir, "src/App.fsproj");

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [fable({ fsproj })],
});