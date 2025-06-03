# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Ruta base
WORKDIR /app

# Copia TODO el repo (incluyendo las librerías)
COPY . .

# Establece el proyecto del worker como raíz de trabajo
WORKDIR /app/Workers/EmailVerificationWorker

# Restaura dependencias (incluye las de la class library referenciada)
RUN dotnet restore

# Publica en carpeta de salida
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime

WORKDIR /app

# Copia el output del build anterior
COPY --from=build /app/Workers/EmailVerificationWorker/out ./

# Ejecuta la app
ENTRYPOINT ["dotnet", "EmailVerificationWorker.dll"]
