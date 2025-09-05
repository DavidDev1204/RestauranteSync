# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos solo los archivos de proyecto primero (para aprovechar la cache de Docker)
COPY RestauranteSync.API/RestauranteSync.API.csproj RestauranteSync.API/
COPY RestauranteSync.Application/RestauranteSync.Application.csproj RestauranteSync.Application/
COPY RestauranteSync.Domain/RestauranteSync.Domain.csproj RestauranteSync.Domain/
COPY RestauranteSync.Infraestructure/RestauranteSync.Infraestructure.csproj RestauranteSync.Infraestructure/

# Restauramos dependencias
RUN dotnet restore RestauranteSync.API/RestauranteSync.API.csproj

# Copiamos todo el código
COPY . .

# Compilamos y publicamos
WORKDIR /src/RestauranteSync.API
RUN dotnet publish -c Release -o /app

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "RestauranteSync.API.dll"]
