# ---------- Angular build ----------
FROM node:22.22.3-alpine AS build

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

RUN npm run build -- \
    --configuration production \
    --output-path=/app/dist


# ---------- Nginx runtime ----------
FROM nginx:1.28-alpine AS final

COPY nginx.conf \
    /etc/nginx/conf.d/default.conf

RUN rm -rf /usr/share/nginx/html/*

COPY --from=build \
    /app/dist/browser \
    /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]