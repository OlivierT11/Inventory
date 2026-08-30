FROM node:22-alpine AS build

WORKDIR /app

# copy lockfile + package.json first to leverage layer cache
COPY package*.json ./

# install dependencies (dev deps required for build)
RUN npm ci --silent

# copy source
COPY . .

# build the app (vite produces /app/dist by default)
RUN npm run build

FROM nginx:alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]