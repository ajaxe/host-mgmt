FROM node:latest as node_builder

ENV NPM_CONFIG_LOGLEVEL warn

RUN mkdir -p /home/app/wwwroot
WORKDIR /home/app/
COPY HostingUserMgmt/VueApp/package.json \
  HostingUserMgmt/VueApp/tsconfig.json \
  HostingUserMgmt/VueApp/yarn.lock \
  ./

RUN yarn global add parcel-bundler && yarn install --production

COPY ./HostingUserMgmt/VueApp/App/ ./App/

# Build all javscript - Final
COPY ./HostingUserMgmt/VueApp/favicon.ico ./wwwroot
RUN parcel build ./App/index.ts -d /home/app/wwwroot

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

# Supress collection of data.
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# Optimize for Docker builder caching by adding projects first.

RUN mkdir -p /home/app/
WORKDIR /home/app/
COPY ./HostingUserMgmt/HostingUserMgmt.csproj  .

WORKDIR /home/app/
RUN dotnet restore ./HostingUserMgmt.csproj

COPY ./HostingUserMgmt  .

RUN dotnet publish -c Release -r linux-musl-x64 -o published

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

# Create a non-root user
RUN addgroup -S app \
  && adduser -S app -G app

WORKDIR /home/app/
COPY --from=builder /home/app/published .
COPY --from=node_builder /home/app/wwwroot ./wwwroot/
RUN chown app:app -R /home/app

VOLUME [ "/home/app/keys" ]
USER app

ENV ASPNETCORE_URLS=http://*:5000 \
  ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

CMD ["dotnet", "./HostingUserMgmt.dll"]