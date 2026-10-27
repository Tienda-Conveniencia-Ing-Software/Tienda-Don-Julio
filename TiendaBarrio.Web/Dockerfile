# Etapa 1: Compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos los .csproj desde la raíz del proyecto
COPY ["TiendaBarrio.Web/TiendaBarrio.Web.csproj", "TiendaBarrio.Web/"]
COPY ["TiendaBarrio/Tienda-Don-Julio.csproj", "TiendaBarrio/"]
RUN dotnet restore "TiendaBarrio.Web/TiendaBarrio.Web.csproj"

# Copiamos todo el código
COPY . .
WORKDIR "/src/TiendaBarrio.Web"
RUN dotnet publish "TiendaBarrio.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TiendaBarrio.Web.dll"]
