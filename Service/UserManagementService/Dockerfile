# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

COPY certs/ums/ums.pfx /https/ums.pfx

# Variables de entorno para HTTPS
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/ums.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=detro123

EXPOSE 7080
EXPOSE 7080


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Service/UserManagementService/UserManagementService.csproj", "Service/UserManagementService/"]
RUN dotnet restore "./Service/UserManagementService/UserManagementService.csproj"
COPY . .
WORKDIR "/src/Service/UserManagementService"
RUN dotnet build "./UserManagementService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserManagementService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagementService.dll"]