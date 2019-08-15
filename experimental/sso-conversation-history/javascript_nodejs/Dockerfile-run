# This is the container for running the demo under Azure Web App
FROM node:12 AS builder-web

# Expose both port 80 and 2222 (SSH for Azure Web App)
EXPOSE 80 2222

WORKDIR /var/

# Extract build image to /var/
ADD build.tgz /var/

# Setup OpenSSH for debugging thru Azure Web App
# https://docs.microsoft.com/en-us/azure/app-service/containers/app-service-linux-ssh-support#ssh-support-with-custom-docker-images
# https://docs.microsoft.com/en-us/azure/app-service/containers/tutorial-custom-docker-image
ENV SSH_PASSWD "root:Docker!"
ENV SSH_PORT 2222
RUN \
  apt-get update \
  && apt-get install -y --no-install-recommends dialog \
  && apt-get update \
  && apt-get install -y --no-install-recommends openssh-server \
  && echo "$SSH_PASSWD" | chpasswd \
  && mv /var/sshd_config /etc/ssh/ \
  && mv /var/init.sh /usr/local/bin/ \
  && chmod u+x /usr/local/bin/init.sh

# Set up entrypoint
ENTRYPOINT init.sh
