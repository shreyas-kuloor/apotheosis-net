FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR src/Apotheosis.Core

# Accept a build argument for configuration, defaulting to Release
ARG BUILD_CONFIGURATION=Release

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c $BUILD_CONFIGURATION -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR src/Apotheosis.Core
COPY --from=build-env /Apotheosis/out .

RUN apt-get update
RUN apt-get -y install libsodium-dev libopus0 libopus-dev opus-tools
RUN apt-get -y install ffmpeg

ENTRYPOINT ["dotnet", "Apotheosis.dll"]