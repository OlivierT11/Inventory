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
ENV VITE_API_URL=${VITE_API_URL}

#temp DEBUG
RUN test -n "$VITE_API_URL" || (echo "VITE_API_URL is missing" && exit 1)  

# build the app (VITE produces /app/dist by default)
RUN echo "Building with VITE_API_URL=${VITE_API_URL}"
RUN npm run build

FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]