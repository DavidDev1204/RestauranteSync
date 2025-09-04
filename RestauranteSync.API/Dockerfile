FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

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
COPY --from=build /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "RestauranteSync.API.dll"]
