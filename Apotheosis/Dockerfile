FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /Apotheosis

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /Apotheosis
COPY --from=build-env /Apotheosis/out .

RUN apt-get update
RUN apt-get -y install libsodium-dev libopus-dev
RUN apt-get -y install ffmpeg

ENTRYPOINT ["dotnet", "Apotheosis.dll"]