# Inventory React

A small inventory management frontend built with React, TypeScript and Vite. Intended to work with an ASP.NET Core products API backend. Includes a production-ready build served by nginx and a Dockerfile for container builds.

## Features
- TypeScript + React
- Vite dev server with HMR
- SPA routing with `react-router-dom`
- Dockerfile for multi-stage production build
- Simple product add / list / edit / delete UI

## Prerequisites
- Node.js 18+ (or use Docker)
- npm
- A backend API at `http://localhost:5293` (or configure via env)

## Quick start (development)
1. Install dependencies
npm ci
2. Start dev server
npm run dev
3. Open `http://localhost:5173` in your browser.

If the HMR websocket fails (common with some editors/OS locks), restart the dev server or add watcher ignores in `vite.config.ts` (example already included).

## Build (production)
Generate a production build:
npm run build

Preview the build locally:
npm run preview
opens at http://localhost:5173 by default


## Docker (production)
The repository includes a multi-stage Dockerfile which builds the app and serves it with nginx.

Build the image:
docker build -t inventory-react .

Run the container (expose port 80):
docker run -p 80:80 inventory-react

Environment note: the Dockerfile supports an optional build argument `VITE_API_URL` used to set a `VITE_API_URL` var for the Vite build if you need to point the client to a different API:
docker build --build-arg VITE_API_URL="https://api.example.com" -t inventory-react .


## Project structure (key files)
- `src/` — React + TypeScript source
- `src/components/` — UI components (`productForm`, `productList`, ...)
- `src/pages/` — routed pages
- `index.html` — app entry
- `src/main.tsx` — React mounting
- `package.json` — npm scripts:
  - `dev` — start Vite dev server
  - `build` — TypeScript build + Vite production build
  - `preview` — preview built assets
- `Dockerfile` — multi-stage build + nginx
- `nginx.conf` — SPA fallback for client-side routing

## Common issues & tips
- If Vite reports file-watch errors on Windows related to Visual Studio (`.vs`), add `.vs` to the watcher ignore list in `vite.config.ts` or add a `.dockerignore` to avoid sending `.vs` into Docker build context.
- If TypeScript errors block `npm run build` during CI/Docker builds, address all TS errors (unused variables, types) or adjust `tsconfig` only with caution.
- Ensure the backend API is reachable and CORS is configured when developing from a different host/port.

## Contributing
- Create feature branches from `main`
- Follow TypeScript and React patterns already used in the codebase
- Open PR with description and testing steps

## License
See the main license file (`LICENSE`) for this repository.
