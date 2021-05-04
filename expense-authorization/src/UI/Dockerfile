# build env

FROM node:lts-alpine AS ng-builder

ARG USE_MOCK_SERVICES

WORKDIR /src
COPY ./angular-eteam/package*.json ./
RUN npm install
COPY ./angular-eteam/ ./
RUN npm run build -- --prod

COPY ./nginx.conf ./

# runtime env
FROM registry.access.redhat.com/ubi8/nginx-118 AS runtime

COPY --from=ng-builder /src/nginx.conf /opt/app-root/etc/nginx.default.d/default.conf

COPY --from=ng-builder /src/dist/angular-eteam/ /usr/share/nginx/html
ENV API_URL=

EXPOSE 8080

RUN printenv
CMD ["sh", "-c", "nginx -g 'daemon off;'"]
