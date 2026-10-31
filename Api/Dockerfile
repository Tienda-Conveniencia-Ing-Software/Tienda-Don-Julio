# ---------- Etapa 1: compilar y publicar ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo los .csproj: Docker cachea el restore si no cambian
COPY ["Api/Api.csproj", "Api/"]
COPY ["TiendaBarrio/Tienda-Don-Julio.csproj", "TiendaBarrio/"]
RUN dotnet restore "Api/Api.csproj"

# Luego el código (el .dockerignore deja fuera bin, obj, .env y *.db)
COPY Api/ Api/
COPY TiendaBarrio/ TiendaBarrio/
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# ---------- Etapa 2: imagen final liviana ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Usuario sin privilegios (viene incluido en las imágenes de .NET 8+)
USER $APP_UID

ENTRYPOINT ["dotnet", "Api.dll"]
