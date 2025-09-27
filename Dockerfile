# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# 1) Copiar el .csproj que está en la raíz y restaurar
COPY PC02.csproj ./
RUN dotnet restore

# 2) Copiar todo el código y publicar
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# 3) Copiar binarios publicados
COPY --from=build-env /app/out .

# 4) Config para Render
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# 5) Arrancar la app (ajusta si tu DLL se llama distinto)
ENTRYPOINT ["dotnet", "PC02.dll"]
