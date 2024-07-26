FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-noble AS build
ARG TARGETARCH
WORKDIR /source

# Copy .csproj file for LupusBytes.CS2.GameStateIntegration.Api
COPY src/LupusBytes.CS2.GameStateIntegration.Api/*.csproj ./src/LupusBytes.CS2.GameStateIntegration.Api/

# Copy .csproj files for projects referenced by LupusBytes.CS2.GameStateIntegration.Api
COPY src/LupusBytes.CS2.GameStateIntegration/*.csproj ./src/LupusBytes.CS2.GameStateIntegration/
COPY src/LupusBytes.CS2.GameStateIntegration.Api.Endpoints/*.csproj ./src/LupusBytes.CS2.GameStateIntegration.Api.Endpoints/
COPY src/LupusBytes.CS2.GameStateIntegration.Contracts/*.csproj ./src/LupusBytes.CS2.GameStateIntegration.Contracts/
COPY src/LupusBytes.CS2.GameStateIntegration.Mqtt/*.csproj ./src/LupusBytes.CS2.GameStateIntegration.Mqtt/
COPY src/LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant/*.csproj ./src/LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant/

# Restore as distinct layer
RUN dotnet restore --arch $TARGETARCH ./src/LupusBytes.CS2.GameStateIntegration.Api

# Copy the rest of the source code
COPY src/ ./src/

# Copy required config files from root directory
COPY .editorconfig .
COPY Directory.Build.props .

# Build the app
RUN dotnet publish --arch $TARGETARCH --no-restore ./src/LupusBytes.CS2.GameStateIntegration.Api/*.csproj -o /app

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./LupusBytes.CS2.GameStateIntegration.Api"]