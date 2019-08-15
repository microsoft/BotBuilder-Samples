# This container is for simplifying CI when using Azure Pipelines

# The first builder image will build HTML and JavaScript code out of the create-react-app project
FROM node:12 AS builder-react
WORKDIR /var/build/react/

# Copy the web app code to /var/build/react/
# We excluded /.env and /node_modules/ via .dockerignore
ADD app/ /var/build/react/

# Doing a fresh "npm install" on build to make sure the image is reproducible
RUN npm ci

# Build the web app code via create-react-app and react-scripts
RUN npm run build

# The second builder image will aggregate all code into a single Docker image for export
FROM node:12

# Copy the bot code to /var/bot/
ADD bot/ /var/build/bot/

# Copy the web server code to /var/web/
ADD rest-api/ /var/build/web/

# Copy SSH configuration and startup script to /var/
# Adopted from https://github.com/Azure-App-Service/node/blob/master/10.14/sshd_config
ADD init.sh /var/build/
ADD sshd_config /var/build/

# Copy static React app to /var/web/public/, to be consumed by web server
COPY --from=builder-react /var/build/react/build/ /var/build/web/public/

# Doing a fresh "npm install" on build to make sure the image is reproducible
WORKDIR /var/build/bot/
RUN npm ci

# Doing a fresh "npm install" on build to make sure the image is reproducible
WORKDIR /var/build/web/
RUN npm ci

# Pack "concurrently" to make sure the image is reproducible
WORKDIR /var/build/
RUN npm install concurrently@4.1.0

# Pack the build content as a "build.tgz" and export it out
RUN tar -cf build.tgz *
