FROM node:lts-alpine AS ng-builder
WORKDIR /src
COPY ./angular-eteam/package*.json ./
RUN npm install
COPY ./angular-eteam/ .
RUN npm run build -- --prod

FROM caddy:alpine as final
# for testing, install curl
RUN apk --no-cache add tini git openssh-client \
    && apk --no-cache add --virtual devs tar curl
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=ng-builder /src/dist/angular-eteam/ /site
ENV API_URL=
EXPOSE 2015