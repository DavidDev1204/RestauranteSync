FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto específico
COPY ["RestauranteSync.API.csproj", "./"]
RUN dotnet restore "RestauranteSync.API.csproj"

# Copiar todo el código fuente
COPY . ./
RUN dotnet build "RestauranteSync.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RestauranteSync.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configuración para Railway
EXPOSE $PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Punto de entrada con el nombre correcto de la DLL
ENTRYPOINT ["dotnet", "RestauranteSync.API.dll"]