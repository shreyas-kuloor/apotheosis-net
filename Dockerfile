FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /src

# Accept a build argument for configuration, defaulting to Release
ARG BUILD_CONFIGURATION=Release

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish src/Apotheosis.Server/Apotheosis.Server.csproj -c $BUILD_CONFIGURATION -o /out/Apotheosis.Server

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /out/Apotheosis.Server .

RUN apt-get update
RUN apt-get -y install libsodium-dev libopus0 libopus-dev opus-tools
RUN apt-get -y install ffmpeg

ENTRYPOINT ["dotnet", "Apotheosis.Server.dll"]