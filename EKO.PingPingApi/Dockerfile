# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV DOTNET_URLS=http://+:5005

# This stage is used to build the service project
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
#COPY ["EKO.PingPingApi/EKO.PingPingApi.csproj", "EKO.PingPingApi/"]
#COPY ["EKO.PingPingApi.Infrastructure/EKO.PingPingApi.Infrastructure.csproj", "EKO.PingPingApi.Infrastructure/"]
#COPY ["EKO.PingPingApi.Shared/EKO.PingPingApi.Shared.csproj", "EKO.PingPingApi.Shared/"]
COPY . .
RUN dotnet restore -r linux-arm64 "./EKO.PingPingApi/EKO.PingPingApi.csproj"
COPY . .
WORKDIR "/src/EKO.PingPingApi"
RUN dotnet build -r linux-arm64 "./EKO.PingPingApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -r linux-arm64 "./EKO.PingPingApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EKO.PingPingApi.dll"]