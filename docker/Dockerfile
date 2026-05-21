# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["EncuentraTuHogar.csproj", "./"]
RUN dotnet restore "EncuentraTuHogar.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "EncuentraTuHogar.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "EncuentraTuHogar.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Render usa PORT, ASP.NET Core 8+ usa 8080 por defecto.
# Configuramos ASPNETCORE_URLS explícitamente para enlazar a todos los puertos 8080.
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "EncuentraTuHogar.dll"]
