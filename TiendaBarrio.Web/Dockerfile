# Etapa 1: Compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos TODO el código primero (así no falla si la ruta es distinta)
COPY . .

# Entramos a la carpeta de la web y restauramos
WORKDIR "/src/TiendaBarrio.Web"
RUN dotnet restore "TiendaBarrio.Web.csproj"

# Compilamos
RUN dotnet publish "TiendaBarrio.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TiendaBarrio.Web.dll"]
