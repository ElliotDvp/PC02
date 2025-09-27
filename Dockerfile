# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copiar solución y proyecto, restaurar paquetes
COPY *.sln ./
COPY PC02/PC02.csproj ./PC02/
RUN dotnet restore

# Copiar todo el código y publicar
COPY . ./
RUN dotnet publish PC02/PC02.csproj -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Traer artefactos compilados
COPY --from=build-env /app/out .

# Variables de entorno para Render
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# El entrypoint arranca la aplicación
ENTRYPOINT ["dotnet", "PC02.dll"]
