# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

COPY certs/hangfire/hanguser.pfx /https/hanguser.pfx

# Variables de entorno para HTTPS
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/hanguser.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=pass123

EXPOSE 3434
EXPOSE 3435

# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server/HangfireUserServer/HangfireUserServer.csproj", "Server/HangfireUserServer/"]
RUN dotnet restore "./Server/HangfireUserServer/HangfireUserServer.csproj"
COPY . .
WORKDIR "/src/Server/HangfireUserServer"
#dotnet ef
RUN dotnet tool install --global dotnet-ef 
ENV PATH="$PATH:/root/.dotnet/tools"
#build
RUN dotnet build "./HangfireUserServer.csproj" -c $BUILD_CONFIGURATION -o /app/build
#execute dotnet ef
CMD dotnet ef database update --environment Production --project src/TASK-FLOW.NET

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HangfireUserServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HangfireUserServer.dll"]