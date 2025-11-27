# https://hub.docker.com/r/microsoft/devcontainers-dotnet
# You can update the version of the base image with .NET and SDK only checking it together with the related VSCode extensions
# Please see the detailed comment in devcontainer.json
FROM mcr.microsoft.com/devcontainers/dotnet:1.5.5-9.0-noble

# installing EntityFramework Core tooling for migrations
RUN dotnet tool install --global dotnet-ef --version 9.0.7 && \
# installing tooling for tests coverage and its reporting
    dotnet tool install --global dotnet-coverage --version 18.1.0 && \
    dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.4.18

# https://stackoverflow.com/a/72727052
# we need to manually add it to PATH otherwise it cannot find dotnet ef tool
ENV PATH="$PATH:/root/.dotnet/tools"

# installing jre, curl, and unzip for Karate 
# https://stackoverflow.com/a/44154721
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive \
    apt-get -y install --no-install-recommends \ 
    curl unzip default-jre-headless

RUN curl -o /karate.jar -L 'https://github.com/intuit/karate/releases/download/v1.5.1/karate-1.5.1.jar'

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1
