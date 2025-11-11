# Production-ready Dockerfile for EventLink
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files (check tên folder chính xác!)
COPY ["EventLink/EventLink.csproj", "EventLink/"]
COPY ["EventLink_Repositories/EventLink_Repositories.csproj", "EventLink_Repositories/"]
COPY ["Eventlink_Services/Eventlink_Services.csproj", "Eventlink_Services/"]

# Restore dependencies
RUN dotnet restore "EventLink/EventLink.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/EventLink"
RUN dotnet build "EventLink.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EventLink.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set production environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EventLink.dll"]