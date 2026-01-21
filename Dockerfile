# UMO API - Dockerfile
# Multi-stage build für optimale Größe

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiere csproj und restore dependencies
COPY ["UMOApi.csproj", "./"]
RUN dotnet restore "UMOApi.csproj"

# Kopiere den Rest des Codes und baue
COPY . .
RUN dotnet build "UMOApi.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "UMOApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage - kleines Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Erstelle Verzeichnis für SQLite Datenbank
RUN mkdir -p /app/data

# Kopiere veröffentlichte Anwendung
COPY --from=publish /app/publish .

# Umgebungsvariablen - Render.com verwendet Port 10000
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Port freigeben (Render.com Standard)
EXPOSE 10000

# Starte die Anwendung
ENTRYPOINT ["dotnet", "UMOApi.dll"]
