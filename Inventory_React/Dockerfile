FROM node:22-alpine AS build

WORKDIR /app

# copy lockfile + package.json first to leverage layer cache
COPY package*.json ./

# install dependencies (dev deps required for build)
RUN npm ci --silent

# copy source
COPY . .

# use the vars in .env.production or .env.dev
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

# build the app (vite produces /app/dist by default)
RUN npm run build

FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]