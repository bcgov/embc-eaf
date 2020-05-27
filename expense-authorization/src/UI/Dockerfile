FROM node:lts-alpine AS ng-builder
WORKDIR /src
COPY ./angular-eteam/package*.json ./
RUN npm install
COPY ./angular-eteam/ .
RUN npm run build -- --prod

#FROM caddy:builder AS caddy-builder
#RUN caddy-builder

FROM caddy:alpine as final
#COPY --from=caddy-builder /usr/bin/caddy /usr/bin/caddy
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/angular-eteam/ /site
ENV API_URL=
EXPOSE 2015