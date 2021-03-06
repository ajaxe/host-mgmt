# Private Hosting User Management

## Introduction

Simple interface for registering new users for the [Private Hosting](http://apogee-dev.com/projects/self-hosting/) infrastructure. The application will allow users to generate username/password credentials. The credentials will be accessible to a stand alone authentication service. The standalone authentication service will be used to handle authentication requests forwarded by Traefik.

## Setup Configuration

The project relies on `secrets.{env}.json` one-level outside the workspace. The `secrets.{env}.json` contains the secrets for external identity providers. Where `{env}` is environment specific keyword like `Development` or `Production`.

## Docker Image

Docker image build:

`docker build --tag docker-registry.apogee-dev.com/host-mgmt:latest .`

Docker image push

`docker push docker-registry.apogee-dev.com/host-mgmt:latest`

## Docker Compose File

[docker-compose.yml](https://github.com/ajaxe/private-hosting/blob/master/host-mgmt/docker-compose.yml) contains an example for deploying the application.
