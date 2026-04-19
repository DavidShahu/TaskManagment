# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies
COPY package*.json .
RUN npm ci

# Copy and build
COPY . .
RUN npm run build

# Runtime stage — serve with nginx
FROM nginx:alpine AS runtime

# Copy built Angular app
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

# Copy nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80