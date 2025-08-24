# Usar .NET 9 oficial
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["*.csproj", "./"]
RUN dotnet restore

# Copiar código fuente y compilar
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configuración para Railway
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
ENTRYPOINT ["dotnet", "TuProyecto.dll"]